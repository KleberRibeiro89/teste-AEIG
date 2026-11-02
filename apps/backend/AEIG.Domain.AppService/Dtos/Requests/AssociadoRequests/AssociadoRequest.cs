namespace AEIG.Domain.AppService.Dtos.Requests.AssociadoRequests;

public class AssociadoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorMensalidade { get; set; }
    public int DiaVencimento { get; set; }
}
