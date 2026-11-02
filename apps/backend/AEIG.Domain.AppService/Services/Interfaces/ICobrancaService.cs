using AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Services.Interfaces;
using AEIG.Domain.Entities;

namespace AEIG.Domain.AppService.Interfaces;

public interface ICobrancaService : IServiceBase<Cobranca, CobrancaRequest, CobrancaResponse>
{
    Task<PagamentoResponse?> PagarAsync(Guid navigationId, PagamentoRequest request);
}
