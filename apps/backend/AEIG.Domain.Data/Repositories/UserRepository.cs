using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email) =>
        DbSet.FirstOrDefaultAsync(u => u.Email == email && u.Active);

    public Task<bool> ExistsByEmailAsync(string email) =>
        DbSet.AnyAsync(u => u.Email == email && u.Active);
}
