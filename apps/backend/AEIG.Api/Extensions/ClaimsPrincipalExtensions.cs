using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AEIG.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        user.FindUserId() ?? throw new UnauthorizedAccessException("Token sem identificação do usuário.");

    public static Guid? FindUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
