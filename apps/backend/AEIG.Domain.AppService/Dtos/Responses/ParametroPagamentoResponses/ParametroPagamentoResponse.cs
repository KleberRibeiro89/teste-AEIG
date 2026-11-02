namespace AEIG.Domain.AppService.Dtos.Responses.ParametroPagamentoResponses;

public class ParametroPagamentoResponse
{
    public Guid NavigationId { get; set; }
    public decimal MultaPercentual { get; set; }
    public decimal JurosDiarioPercentual { get; set; }
}
