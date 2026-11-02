using AEIG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data;

public class AEIGDbContext : DbContext
{
    public AEIGDbContext(DbContextOptions<AEIGDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        RegisterAllEntities(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AEIGDbContext).Assembly);
    }

    private static void RegisterAllEntities(ModelBuilder modelBuilder)
    {
        var entities = typeof(BaseEntity).Assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(BaseEntity).IsAssignableFrom(type));

        foreach (var entity in entities)
            modelBuilder.Entity(entity);
    }
}
