using AEIG.Domain.Entities;
using AEIG.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AEIG.Domain.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AEIGDbContext>();

        await context.Database.MigrateAsync();
        await SeedUsersAsync(context);
        await SeedStatusesAsync(context);
        await SeedParametrosPagamentoAsync(context);
    }

    public static async Task SeedUsersAsync(AEIGDbContext context)
    {
        var passwordHash = PasswordHasher.Hash("123456");
        var now = DateTime.UtcNow;
        var users = new (string Name, string Email)[]
        {
            ("Kleber Ribeiro", "kleber.ribeiro89@gmail.com"),
            ("Admin", "admin@aeig.com.br")
        };

        var emails = users.Select(user => user.Email).ToArray();
        var existingEmails = await context.Set<User>()
            .Where(u => emails.Contains(u.Email))
            .Select(u => u.Email)
            .ToListAsync();

        var missing = users
            .Where(user => !existingEmails.Contains(user.Email))
            .Select(user => new User
            {
                NavigationId = Guid.NewGuid(),
                Nome = user.Name,
                Email = user.Email,
                HashSenha = passwordHash,
                CreatedAt = now,
                CreatedBy = Guid.Empty,
                Active = true
            })
            .ToList();

        if (missing.Count == 0)
            return;

        await context.Set<User>().AddRangeAsync(missing);
        await context.SaveChangesAsync();
    }

    public static async Task SeedStatusesAsync(AEIGDbContext context)
    {
        var names = new[] { "Pendente", "Em Atraso", "Pago", "Pago com Juros" };
        var existing = await context.Set<Status>()
            .Where(status => names.Contains(status.Nome))
            .Select(status => status.Nome)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var missing = names
            .Where(name => !existing.Contains(name))
            .Select(name => new Status
            {
                NavigationId = Guid.NewGuid(),
                Nome = name,
                CreatedAt = now,
                CreatedBy = Guid.Empty,
                Active = true
            })
            .ToList();

        if (missing.Count == 0)
            return;

        await context.Set<Status>().AddRangeAsync(missing);
        await context.SaveChangesAsync();
    }

    public static async Task SeedParametrosPagamentoAsync(AEIGDbContext context)
    {
        var exists = await context.Set<ParametroPagamento>().AnyAsync(p => p.Active);
        if (exists)
            return;

        var now = DateTime.UtcNow;
        await context.Set<ParametroPagamento>().AddAsync(new ParametroPagamento
        {
            NavigationId = Guid.NewGuid(),
            MultaPercentual = 2m,
            JurosDiarioPercentual = 0.033m,
            CreatedAt = now,
            CreatedBy = Guid.Empty,
            Active = true
        });
        await context.SaveChangesAsync();
    }
}
