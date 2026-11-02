namespace AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;

public class PagamentoResponse
{
    public Guid NavigationId { get; set; }
    public DateOnly DataPagamento { get; set; }
    public int DiasAtraso { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public decimal ValorOriginal { get; set; }
    public decimal ValorPago { get; set; }
    public string? NomeStatus { get; set; }
}
