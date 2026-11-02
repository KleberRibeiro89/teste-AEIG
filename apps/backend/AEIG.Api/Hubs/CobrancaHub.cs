using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AEIG.Api.Hubs;

[Authorize]
public class CobrancaHub : Hub
{
}
