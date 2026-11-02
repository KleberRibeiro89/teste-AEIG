namespace AEIG.Domain.AppService.Services.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }
}
