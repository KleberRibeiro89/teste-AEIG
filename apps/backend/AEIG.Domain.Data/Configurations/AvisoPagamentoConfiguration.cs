using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEIG.Domain.Data.Configurations;

public class AvisoPagamentoConfiguration : IEntityTypeConfiguration<AvisoPagamento>
{
    public void Configure(EntityTypeBuilder<AvisoPagamento> builder)
    {
        builder.Property(a => a.IdEvento).HasMaxLength(100).IsRequired();
        builder.Property(a => a.NossoNumero).HasMaxLength(20).IsRequired();
        builder.Property(a => a.NomeStatus).HasMaxLength(50);
        builder.Property(a => a.Mensagem).HasMaxLength(200).IsRequired();
        builder.Property(a => a.ValorPago).HasPrecision(10, 2);
        builder.HasIndex(a => a.IdEvento).IsUnique();
        builder.HasIndex(a => a.NavigationId).IsUnique();
    }
}
