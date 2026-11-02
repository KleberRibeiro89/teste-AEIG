namespace AEIG.Domain.AppService.Dtos.Responses.DashboardResponses;

public class DashboardResponse
{
    public int Associados { get; set; }
    public List<DashboardStatusResponse> Cobrancas { get; set; } = [];
}

public class DashboardStatusResponse
{
    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorPago { get; set; }
}
