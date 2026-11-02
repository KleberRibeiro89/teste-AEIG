    namespace AEIG.Domain.Entities;

public record ParametroPagamento : BaseEntity
{
    public decimal MultaPercentual { get; set; }
    public decimal JurosDiarioPercentual { get; set; }
}
