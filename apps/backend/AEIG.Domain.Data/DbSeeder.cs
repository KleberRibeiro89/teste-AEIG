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
                Name = user.Name,
                Email = user.Email,
                PasswordHash = passwordHash,
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
}
