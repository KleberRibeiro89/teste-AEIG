using AEIG.Api.Hubs;
using AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;
using AEIG.Domain.AppService.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AEIG.Api.Notifications;

public class CobrancaNotificador : ICobrancaNotificador
{
    private readonly IHubContext<CobrancaHub> _hub;
    private readonly ILogger<CobrancaNotificador> _logger;

    public CobrancaNotificador(IHubContext<CobrancaHub> hub, ILogger<CobrancaNotificador> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task NotificarBaixaAsync(CobrancaBaixadaNotificacao aviso)
    {
        try
        {
            await _hub.Clients.All.SendAsync("CobrancaBaixada", aviso);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível notificar a baixa da cobrança {NossoNumero}.", aviso.NossoNumero);
        }
    }
}
