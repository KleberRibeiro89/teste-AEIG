using System.Text.RegularExpressions;
using AEIG.Domain;
using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Dtos.Responses.DashboardResponses;
using AEIG.Domain.AppService.Extensions;
using AEIG.Domain.AppService.Interfaces;
using AEIG.Domain.AppService.Services.Interfaces;
using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace AEIG.Domain.AppService.Services;

public class CobrancaService : ServiceBase<Cobranca, CobrancaRequest, CobrancaResponse>, ICobrancaService
{
    public const string PendingStatusName = "Pendente";
    public const string PaidStatusName = "Pago";
    public const string PaidWithInterestStatusName = "Pago com Juros";

    private static readonly string[] OrdemStatus = ["Pendente", "Em Atraso", "Pago", "Pago com Juros"];

    private readonly ICobrancaRepository _cobrancaRepository;
    private readonly IAssociadoRepository _associadoRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IParametroPagamentoRepository _parametroPagamentoRepository;
    private readonly IAvisoPagamentoRepository _avisoRepository;
    private readonly ICobrancaNotificador _notificador;
    private readonly IValidator<AvisoPagamentoRequest> _avisoValidator;

    public CobrancaService(
        ICobrancaRepository repository,
        IAssociadoRepository associadoRepository,
        IStatusRepository statusRepository,
        IParametroPagamentoRepository parametroPagamentoRepository,
        IAvisoPagamentoRepository avisoRepository,
        ICobrancaNotificador notificador,
        IMapper mapper,
        ICurrentUser currentUser,
        IValidator<Cobranca> validator,
        IValidator<AvisoPagamentoRequest> avisoValidator)
        : base(repository, mapper, currentUser, validator)
    {
        _cobrancaRepository = repository;
        _associadoRepository = associadoRepository;
        _statusRepository = statusRepository;
        _parametroPagamentoRepository = parametroPagamentoRepository;
        _avisoRepository = avisoRepository;
        _notificador = notificador;
        _avisoValidator = avisoValidator;
    }

    protected override async Task ValidateCreateAsync(Cobranca entity)
    {
        await PrepareAsync(entity, generateNossoNumero: true);
        await base.ValidateCreateAsync(entity);
        await EnsureChargeRulesAsync(entity);
    }

    protected override async Task ValidateUpdateAsync(Cobranca entity)
    {
        await PrepareAsync(entity, generateNossoNumero: false);
        await base.ValidateUpdateAsync(entity);
        await EnsureChargeRulesAsync(entity);
    }

    private async Task PrepareAsync(Cobranca entity, bool generateNossoNumero)
    {
        if (generateNossoNumero && string.IsNullOrWhiteSpace(entity.NossoNumero))
            entity.NossoNumero = await GenerateNossoNumeroAsync(entity.NavigationId);

        if (generateNossoNumero)
            await PreencherValoresAsync(entity);

        if (entity.IdStatus == 0)
        {
            var pending = await _statusRepository.GetActiveByNameAsync(PendingStatusName);
            if (pending is null)
                throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdStatus), "Status Pendente não encontrado.")]);
            entity.IdStatus = pending.Id;
        }
    }

    private async Task PreencherValoresAsync(Cobranca entity)
    {
        if (entity.Valor > 0 && entity.DataVencimento != default)
            return;

        var associado = await _associadoRepository.GetActiveByIdAsync(entity.IdAssociado);
        if (associado is null)
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdAssociado), "Associado não encontrado.")]);

        if (entity.Valor <= 0)
            entity.Valor = associado.ValorMensalidade;

        if (entity.DataVencimento == default && MesReferenciaValido(entity.MesReferencia))
            entity.DataVencimento = CalcularDataVencimento(entity.MesReferencia, associado.DiaVencimento);
    }

    private static bool MesReferenciaValido(string mesReferencia) =>
        Regex.IsMatch(mesReferencia, @"^\d{4}-(0[1-9]|1[0-2])$");

    private static DateOnly CalcularDataVencimento(string mesReferencia, int diaVencimento)
    {
        var ano = int.Parse(mesReferencia.AsSpan(0, 4));
        var mes = int.Parse(mesReferencia.AsSpan(5, 2));
        var dia = Math.Clamp(diaVencimento, 1, DateTime.DaysInMonth(ano, mes));
        return new DateOnly(ano, mes, dia);
    }

    private async Task EnsureChargeRulesAsync(Cobranca entity)
    {
        if (!await _associadoRepository.ExistsActiveAsync(entity.IdAssociado))
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdAssociado), "Associado não encontrado.")]);

        if (!await _statusRepository.ExistsActiveAsync(entity.IdStatus))
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdStatus), "Status não encontrado.")]);

        if (await _cobrancaRepository.ExistsInMonthAsync(entity.IdAssociado, entity.MesReferencia, entity.NavigationId))
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.MesReferencia), "Já existe uma cobrança para este associado neste mês.")]);

        if (await _cobrancaRepository.NossoNumeroExistsAsync(entity.NossoNumero, entity.NavigationId))
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.NossoNumero), "Nosso número já utilizado.")]);
    }

    private async Task<string> GenerateNossoNumeroAsync(Guid navigationId)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var number = $"{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(0, 1000000):000000}";
            if (!await _cobrancaRepository.NossoNumeroExistsAsync(number, navigationId))
                return number;
        }

        throw new ValidationException([new ValidationFailure(nameof(Cobranca.NossoNumero), "Não foi possível gerar o nosso número.")]);
    }

    public async Task<PagamentoResponse?> PagarAsync(Guid navigationId, PagamentoRequest request)
    {
        var cobranca = await Repository.GetByIdAsync(navigationId);
        if (cobranca is null)
            return null;

        var dataPagamento = request.DataPagamento ?? DateOnly.FromDateTime(DateTime.Today);
        if (dataPagamento > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException([new ValidationFailure(nameof(PagamentoRequest.DataPagamento), "A data de pagamento não pode ser futura.")]);

        var pago = await ObterStatusAsync(PaidStatusName);
        var pagoComJuros = await ObterStatusAsync(PaidWithInterestStatusName);
        if (cobranca.IdStatus == pago.Id || cobranca.IdStatus == pagoComJuros.Id)
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdStatus), "Cobrança já está paga.")]);

        var parametro = await _parametroPagamentoRepository.GetAtivoAsync();
        if (parametro is null)
            throw new ValidationException([new ValidationFailure(nameof(ParametroPagamento), "Parâmetros de pagamento não encontrados.")]);

        var valorOriginal = cobranca.Valor.RoundMoney();
        var diasAtraso = Math.Max(0, dataPagamento.DayNumber - cobranca.DataVencimento.DayNumber);
        if (request.Juros is < 0)
            throw new ValidationException([new ValidationFailure(nameof(PagamentoRequest.Juros), "Os juros não podem ser negativos.")]);

        var multa = diasAtraso == 0
            ? 0
            : (valorOriginal * parametro.MultaPercentual / 100m).RoundMoney();
        var juros = (request.Juros ?? (diasAtraso == 0
            ? 0
            : valorOriginal * parametro.JurosDiarioPercentual / 100m * diasAtraso)).RoundMoney();
        var status = diasAtraso > 0 || juros > 0 ? pagoComJuros : pago;

        var valorPago = (valorOriginal + multa + juros).RoundMoney();
        cobranca.DataPagamento = dataPagamento;
        cobranca.ValorPago = valorPago;
        cobranca.IdStatus = status.Id;
        await UpdateAsync(cobranca);
        await _notificador.NotificarBaixaAsync(new CobrancaBaixadaNotificacao
        {
            NavigationId = cobranca.NavigationId,
            NossoNumero = cobranca.NossoNumero,
            NomeStatus = status.Nome,
            ValorPago = valorPago,
            DataPagamento = dataPagamento
        });

        return new PagamentoResponse
        {
            NavigationId = cobranca.NavigationId,
            DataPagamento = dataPagamento,
            DiasAtraso = diasAtraso,
            Multa = multa,
            Juros = juros,
            ValorOriginal = valorOriginal,
            ValorPago = valorPago,
            NomeStatus = status.Nome
        };
    }

    public async Task<DashboardResponse> ObterDashboardAsync()
    {
        var associados = await _associadoRepository.CountActiveAsync();
        var resumo = await _cobrancaRepository.GetResumoPorStatusAsync();
        var recebidos = await _cobrancaRepository.GetRecebidosPorMesAsync();
        var atrasos = await _cobrancaRepository.GetAtrasosPorMesAsync();
        var porNome = resumo.ToDictionary(item => item.NomeStatus, StringComparer.OrdinalIgnoreCase);

        var cobrancas = OrdemStatus.Select(nome =>
        {
            porNome.TryGetValue(nome, out var item);
            return new DashboardStatusResponse
            {
                Nome = nome,
                Quantidade = item?.Quantidade ?? 0,
                Valor = item?.Valor ?? 0,
                ValorPago = item?.ValorPago ?? 0
            };
        }).ToList();

        foreach (var extra in resumo.Where(item => !OrdemStatus.Contains(item.NomeStatus, StringComparer.OrdinalIgnoreCase)))
        {
            cobrancas.Add(new DashboardStatusResponse
            {
                Nome = extra.NomeStatus,
                Quantidade = extra.Quantidade,
                Valor = extra.Valor,
                ValorPago = extra.ValorPago
            });
        }

        return new DashboardResponse
        {
            Associados = associados,
            Cobrancas = cobrancas,
            Recebidos = MapMeses(recebidos),
            Atrasos = MapMeses(atrasos)
        };
    }

    public async Task<WebhookPagamentoResult> ReceberAvisoAsync(AvisoPagamentoRequest request)
    {
        request.IdEvento = request.IdEvento.Trim();
        request.NossoNumero = request.NossoNumero.Trim();
        await _avisoValidator.ValidateAndThrowAsync(request);

        WebhookPagamentoResult? resultado = null;
        try
        {
            await _avisoRepository.ExecuteInTransactionAsync(async () =>
            {
                resultado = await ProcessarAvisoAsync(request);
            });
        }
        catch (AvisoDuplicadoException)
        {
            var existente = await _avisoRepository.GetByIdEventoAsync(request.IdEvento);
            if (existente is null)
                throw;

            return AvisoEncontrado(MapAviso(existente, jaProcessado: true));
        }

        return resultado ?? new WebhookPagamentoResult();
    }

    private async Task<WebhookPagamentoResult> ProcessarAvisoAsync(AvisoPagamentoRequest request)
    {
        var existente = await _avisoRepository.GetByIdEventoAsync(request.IdEvento);
        if (existente is not null)
            return AvisoEncontrado(MapAviso(existente, jaProcessado: true));

        var cobranca = await _cobrancaRepository.GetActiveByNossoNumeroAsync(request.NossoNumero);
        if (cobranca is null)
            return new WebhookPagamentoResult();

        if (JaPaga(cobranca))
        {
            var aviso = new AvisoPagamento
            {
                IdEvento = request.IdEvento,
                NossoNumero = cobranca.NossoNumero,
                DataPagamento = cobranca.DataPagamento ?? request.DataPagamento,
                NomeStatus = cobranca.Status?.Nome,
                ValorPago = cobranca.ValorPago,
                Mensagem = "Cobrança já está paga."
            };
            await _avisoRepository.RegistrarAsync(aviso);
            return AvisoEncontrado(MapAviso(aviso, jaProcessado: false));
        }

        var pagamento = await PagarAsync(cobranca.NavigationId, new PagamentoRequest
        {
            DataPagamento = request.DataPagamento
        });
        if (pagamento is null)
            return new WebhookPagamentoResult();

        var registrado = new AvisoPagamento
        {
            IdEvento = request.IdEvento,
            NossoNumero = cobranca.NossoNumero,
            DataPagamento = pagamento.DataPagamento,
            NomeStatus = pagamento.NomeStatus,
            ValorPago = pagamento.ValorPago,
            Mensagem = "Pagamento registrado."
        };
        await _avisoRepository.RegistrarAsync(registrado);
        return AvisoEncontrado(MapAviso(registrado, jaProcessado: false));
    }

    private static List<DashboardMesResponse> MapMeses(List<CobrancaMesResumo> meses) =>
        meses
            .OrderBy(item => item.Mes)
            .Select(item => new DashboardMesResponse { Mes = item.Mes, Valor = item.Valor })
            .ToList();

    private static bool JaPaga(Cobranca cobranca) =>
        (cobranca.Status?.Nome ?? string.Empty).StartsWith("Pago", StringComparison.OrdinalIgnoreCase);

    private static WebhookPagamentoResult AvisoEncontrado(AvisoPagamentoResponse resposta) =>
        new() { CobrancaEncontrada = true, Resposta = resposta };

    private static AvisoPagamentoResponse MapAviso(AvisoPagamento aviso, bool jaProcessado) => new()
    {
        IdEvento = aviso.IdEvento,
        NossoNumero = aviso.NossoNumero,
        DataPagamento = aviso.DataPagamento,
        NomeStatus = aviso.NomeStatus,
        ValorPago = aviso.ValorPago,
        JaProcessado = jaProcessado,
        Mensagem = aviso.Mensagem
    };

    private async Task<Status> ObterStatusAsync(string nome)
    {
        var status = await _statusRepository.GetActiveByNameAsync(nome);
        if (status is null)
            throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdStatus), $"Status {nome} não encontrado.")]);
        return status;
    }
}
