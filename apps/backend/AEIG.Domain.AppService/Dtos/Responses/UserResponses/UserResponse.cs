namespace AEIG.Domain.AppService.Dtos.Responses.UserResponses;

public class UserResponse
{
    public Guid NavigationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
}
