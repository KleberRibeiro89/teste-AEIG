namespace AEIG.Domain.Entities;

public record Associado : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorMensalidade { get; set; }
    public int DiaVencimento { get; set; }
}
