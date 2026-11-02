namespace AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;

public class CobrancaRequest
{
    public int IdAssociado { get; set; }
    public string MesReferencia { get; set; } = string.Empty;
    public decimal? Valor { get; set; }
    public decimal? ValorPago { get; set; }
    public DateOnly? DataVencimento { get; set; }
    public int IdStatus { get; set; }
}
