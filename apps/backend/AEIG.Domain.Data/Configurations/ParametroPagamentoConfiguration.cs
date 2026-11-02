using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEIG.Domain.Data.Configurations;

public class ParametroPagamentoConfiguration : IEntityTypeConfiguration<ParametroPagamento>
{
    public void Configure(EntityTypeBuilder<ParametroPagamento> builder)
    {
        builder.Property(p => p.MultaPercentual).HasPrecision(10, 6);
        builder.Property(p => p.JurosDiarioPercentual).HasPrecision(10, 6);
        builder.HasIndex(p => p.NavigationId).IsUnique();
    }
}
