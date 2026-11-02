using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Services.Interfaces;

namespace AEIG.Domain.AppService.Services;

public class NullCobrancaNotificador : ICobrancaNotificador
{
    public Task NotificarBaixaAsync(CobrancaBaixadaNotificacao aviso) => Task.CompletedTask;
}
