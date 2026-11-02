using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class AssociadoRepository : RepositoryBase<Associado>, IAssociadoRepository
{
    public AssociadoRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<Associado?> GetByCpfAsync(string cpf) =>
        DbSet.FirstOrDefaultAsync(a => a.Cpf == cpf && a.Active);

    public Task<Associado?> GetActiveByIdAsync(int id) =>
        DbSet.FirstOrDefaultAsync(a => a.Id == id && a.Active);

    public Task<bool> ExistsActiveAsync(int id) =>
        DbSet.AnyAsync(a => a.Id == id && a.Active);

    public Task<int> CountActiveAsync() =>
        DbSet.CountAsync(a => a.Active);
}
