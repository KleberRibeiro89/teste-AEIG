namespace AEIG.Domain.Entities;

public record Associado : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
}
