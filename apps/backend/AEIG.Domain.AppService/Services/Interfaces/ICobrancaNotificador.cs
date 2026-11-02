using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;

namespace AEIG.Domain.AppService.Services.Interfaces;

public interface ICobrancaNotificador
{
    Task NotificarBaixaAsync(CobrancaBaixadaNotificacao aviso);
}
