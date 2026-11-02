using AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;
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

    public AssociadoService(IAssociadoRepository repository, IMapper mapper, ICurrentUser currentUser, IValidator<Associado> validator)
        : base(repository, mapper, currentUser, validator)
    {
        _associadoRepository = repository;
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
