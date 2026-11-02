using AEIG.Domain.AppService.Dtos.Requests.ParametroPagamentoRequests;
using AEIG.Domain.AppService.Dtos.Responses.ParametroPagamentoResponses;
using AEIG.Domain.AppService.Interfaces;
using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using FluentValidation;

namespace AEIG.Domain.AppService.Services;

public class ParametroPagamentoService : IParametroPagamentoService
{
    private readonly IParametroPagamentoRepository _repository;
    private readonly IValidator<ParametroPagamentoRequest> _validator;

    public ParametroPagamentoService(
        IParametroPagamentoRepository repository,
        IValidator<ParametroPagamentoRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ParametroPagamentoResponse?> GetAtivoAsync()
    {
        var parametro = await _repository.GetAtivoAsync();
        return parametro is null ? null : Map(parametro);
    }

    public async Task<ParametroPagamentoResponse?> AtualizarAsync(ParametroPagamentoRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var parametro = await _repository.GetAtivoAsync();
        if (parametro is null)
            return null;

        parametro.MultaPercentual = request.MultaPercentual;
        parametro.JurosDiarioPercentual = request.JurosDiarioPercentual;
        var updated = await _repository.UpdateAsync(parametro);
        return Map(updated);
    }

    private static ParametroPagamentoResponse Map(ParametroPagamento parametro) => new()
    {
        NavigationId = parametro.NavigationId,
        MultaPercentual = parametro.MultaPercentual,
        JurosDiarioPercentual = parametro.JurosDiarioPercentual
    };
}
