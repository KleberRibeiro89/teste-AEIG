using AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;
using AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;
using AEIG.Domain.AppService.Services.Interfaces;
using AEIG.Domain.Entities;

namespace AEIG.Domain.AppService.Interfaces;

public interface IAssociadoService : IServiceBase<Associado, AssociadoRequest, AssociadoResponse>;
