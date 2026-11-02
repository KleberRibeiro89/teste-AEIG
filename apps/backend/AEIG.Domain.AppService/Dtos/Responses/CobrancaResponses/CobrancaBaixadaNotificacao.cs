namespace AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;

public class CobrancaBaixadaNotificacao
{
    public Guid NavigationId { get; set; }
    public string NossoNumero { get; set; } = string.Empty;
    public string? NomeStatus { get; set; }
    public decimal ValorPago { get; set; }
    public DateOnly DataPagamento { get; set; }
}
