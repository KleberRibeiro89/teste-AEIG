using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class StatusRepository : RepositoryBase<Status>, IStatusRepository
{
    public StatusRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<Status?> GetActiveByNameAsync(string name) =>
        DbSet.FirstOrDefaultAsync(s => s.Active && s.Nome == name);

    public Task<bool> ExistsActiveAsync(int id) =>
        DbSet.AnyAsync(s => s.Active && s.Id == id);
}
