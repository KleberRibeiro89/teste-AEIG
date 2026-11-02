namespace AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;

public class AssociadoRequest
{
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
}
