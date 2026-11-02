using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AEIG.Domain.Data.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.Property(s => s.Nome).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.Nome).IsUnique();
        builder.HasIndex(s => s.NavigationId).IsUnique();
    }
}
