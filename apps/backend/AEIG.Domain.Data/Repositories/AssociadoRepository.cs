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
}
