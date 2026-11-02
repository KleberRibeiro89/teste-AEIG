using AEIG.Api.Extensions;
using AEIG.Domain.AppService.Services.Interfaces;

namespace AEIG.Api.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => _httpContextAccessor.HttpContext?.User.FindUserId();
}
