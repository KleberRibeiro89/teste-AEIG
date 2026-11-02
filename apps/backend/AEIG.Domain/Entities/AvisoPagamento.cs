namespace AEIG.Domain.Entities;

public record AvisoPagamento : BaseEntity
{
    public string IdEvento { get; set; } = string.Empty;
    public string NossoNumero { get; set; } = string.Empty;
    public DateOnly DataPagamento { get; set; }
    public string? NomeStatus { get; set; }
    public decimal ValorPago { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
