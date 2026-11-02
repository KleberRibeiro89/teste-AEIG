using AEIG.Domain.AppService.Dtos.Requests.ParametroPagamentoRequests;
using AEIG.Domain.AppService.Dtos.Responses.ParametroPagamentoResponses;

namespace AEIG.Domain.AppService.Interfaces;

public interface IParametroPagamentoService
{
    Task<ParametroPagamentoResponse?> GetAtivoAsync();
    Task<ParametroPagamentoResponse?> AtualizarAsync(ParametroPagamentoRequest request);
}
