using AEIG.Domain.Entities;

namespace AEIG.Domain.Repositories;

public interface IStatusRepository : IRepositoryBase<Status>
{
    Task<Status?> GetActiveByNameAsync(string name);
    Task<bool> ExistsActiveAsync(int id);
}
