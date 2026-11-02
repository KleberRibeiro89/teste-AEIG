namespace AEIG.Domain.Entities;

public record Status : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
}
