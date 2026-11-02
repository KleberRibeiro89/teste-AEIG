namespace AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;

public class CobrancaResponse
{
    public Guid NavigationId { get; set; }
    public int IdAssociado { get; set; }
    public string NossoNumero { get; set; } = string.Empty;
    public string MesReferencia { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal ValorPago { get; set; }
    public DateOnly DataVencimento { get; set; }
    public DateOnly? DataPagamento { get; set; }
    public int IdStatus { get; set; }
    public string? NomeStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
}
