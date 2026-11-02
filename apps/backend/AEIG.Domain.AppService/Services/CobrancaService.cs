using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
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

    private readonly ICobrancaRepository _cobrancaRepository;
    private readonly IAssociadoRepository _associadoRepository;
    private readonly IStatusRepository _statusRepository;

    public CobrancaService(
        ICobrancaRepository repository,
        IAssociadoRepository associadoRepository,
        IStatusRepository statusRepository,
        IMapper mapper,
        ICurrentUser currentUser,
        IValidator<Cobranca> validator)
        : base(repository, mapper, currentUser, validator)
    {
        _cobrancaRepository = repository;
        _associadoRepository = associadoRepository;
        _statusRepository = statusRepository;
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

        if (entity.IdStatus == 0)
        {
            var pending = await _statusRepository.GetActiveByNameAsync(PendingStatusName);
            if (pending is null)
                throw new ValidationException([new ValidationFailure(nameof(Cobranca.IdStatus), "Status Pendente não encontrado.")]);
            entity.IdStatus = pending.Id;
        }
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
}
