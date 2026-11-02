using AEIG.Domain.Entities;

namespace AEIG.Domain.AppService.Services.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
