using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEIG.Domain.Data.Configurations;

public class AssociadoConfiguration : IEntityTypeConfiguration<Associado>
{
    public void Configure(EntityTypeBuilder<Associado> builder)
    {
        builder.Property(a => a.Name).HasMaxLength(200);
        builder.Property(a => a.Cpf).HasMaxLength(11);
        builder.Property(a => a.MonthlyFee).HasPrecision(10, 2);
        builder.HasIndex(a => a.NavigationId).IsUnique();
    }
}
