namespace AEIG.Domain.Entities;

public record Cobranca : BaseEntity
{
    public int IdAssociado { get; set; }
    public Associado? Associado { get; set; }
    public string NossoNumero { get; set; } = string.Empty;
    public string MesReferencia { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal ValorPago { get; set; }
    public DateOnly DataVencimento { get; set; }
    public DateOnly? DataPagamento { get; set; }
    public int IdStatus { get; set; }
    public Status? Status { get; set; }
}
