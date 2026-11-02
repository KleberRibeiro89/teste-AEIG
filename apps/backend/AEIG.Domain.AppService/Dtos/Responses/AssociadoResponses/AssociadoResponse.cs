namespace AEIG.Domain.AppService.Dtos.Responses.AssociadoResponses;

public class AssociadoResponse
{
    public Guid NavigationId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorMensalidade { get; set; }
    public int DiaVencimento { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
}
