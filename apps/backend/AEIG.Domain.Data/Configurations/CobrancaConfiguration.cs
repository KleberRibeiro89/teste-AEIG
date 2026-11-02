using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEIG.Domain.Data.Configurations;

public class CobrancaConfiguration : IEntityTypeConfiguration<Cobranca>
{
    public void Configure(EntityTypeBuilder<Cobranca> builder)
    {
        builder.Property(c => c.NossoNumero).HasMaxLength(20).IsRequired();
        builder.Property(c => c.MesReferencia).HasMaxLength(7).IsRequired();
        builder.Property(c => c.Valor).HasPrecision(10, 2);
        builder.Property(c => c.ValorPago).HasPrecision(10, 2);
        builder.HasIndex(c => c.NossoNumero).IsUnique();
        builder.HasIndex(c => c.NavigationId).IsUnique();

        builder.HasOne(c => c.Associado)
            .WithMany()
            .HasForeignKey(c => c.IdAssociado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Status)
            .WithMany()
            .HasForeignKey(c => c.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
