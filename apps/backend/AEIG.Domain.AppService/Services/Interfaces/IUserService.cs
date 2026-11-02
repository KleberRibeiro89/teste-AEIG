using AEIG.Domain.AppService.Dtos.Requests.UserRequests;
using AEIG.Domain.AppService.Dtos.Responses.UserResponses;
using AEIG.Domain.AppService.Services.Interfaces;
using AEIG.Domain.Entities;

namespace AEIG.Domain.AppService.Interfaces;

public interface IUserService : IServiceBase<User, UserRequest, UserResponse>
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
