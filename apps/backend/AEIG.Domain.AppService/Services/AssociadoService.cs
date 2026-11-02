using AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;
using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;
using AEIG.Domain.AppService.Interfaces;
using AEIG.Domain.AppService.Services.Interfaces;
using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace AEIG.Domain.AppService.Services;

public class AssociadoService : ServiceBase<Associado, AssociadoRequest, AssociadoResponse>, IAssociadoService
{
    private readonly IAssociadoRepository _associadoRepository;
    private readonly ICobrancaService _cobrancaService;

    public AssociadoService(
        IAssociadoRepository repository,
        ICobrancaService cobrancaService,
        IMapper mapper,
        ICurrentUser currentUser,
        IValidator<Associado> validator)
        : base(repository, mapper, currentUser, validator)
    {
        _associadoRepository = repository;
        _cobrancaService = cobrancaService;
    }

    public override async Task<AssociadoResponse> CreateAsync(Associado entity, bool isActive = true)
    {
        var created = await base.CreateAsync(entity, isActive);
        await GerarCobrancasAsync(entity);
        return created;
    }

    private async Task GerarCobrancasAsync(Associado associado)
    {
        var inicio = PrimeiroMesReferencia(DateOnly.FromDateTime(DateTime.Today), associado.DiaVencimento);
        for (var i = 0; i < 12; i++)
        {
            var mes = inicio.AddMonths(i);
            await _cobrancaService.CreateAsync(new CobrancaRequest
            {
                IdAssociado = associado.Id,
                MesReferencia = $"{mes.Year}-{mes.Month:00}"
            });
        }
    }

    private static DateOnly PrimeiroMesReferencia(DateOnly hoje, int diaVencimento)
    {
        var dia = Math.Clamp(diaVencimento, 1, DateTime.DaysInMonth(hoje.Year, hoje.Month));
        var inicio = new DateOnly(hoje.Year, hoje.Month, 1);
        if (new DateOnly(hoje.Year, hoje.Month, dia) <= hoje)
            inicio = inicio.AddMonths(1);
        return inicio;
    }

    protected override async Task ValidateCreateAsync(Associado entity)
    {
        await base.ValidateCreateAsync(entity);
        await EnsureCpfIsUniqueAsync(entity);
    }

    protected override async Task ValidateUpdateAsync(Associado entity)
    {
        await base.ValidateUpdateAsync(entity);
        await EnsureCpfIsUniqueAsync(entity);
    }

    private async Task EnsureCpfIsUniqueAsync(Associado entity)
    {
        var existing = await _associadoRepository.GetByCpfAsync(entity.Cpf);
        if (existing is not null && existing.NavigationId != entity.NavigationId)
            throw new ValidationException([new ValidationFailure(nameof(Associado.Cpf), "Já existe um associado com este CPF.")]);
    }
}
