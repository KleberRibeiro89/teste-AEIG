namespace AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;

public class AssociadoResponse
{
    public Guid NavigationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
}
