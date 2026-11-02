namespace AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;

public class PagamentoRequest
{
    public DateOnly? DataPagamento { get; set; }
    public decimal? Juros { get; set; }
}
