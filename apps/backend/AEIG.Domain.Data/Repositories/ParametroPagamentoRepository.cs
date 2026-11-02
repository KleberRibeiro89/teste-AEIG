using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class ParametroPagamentoRepository : RepositoryBase<ParametroPagamento>, IParametroPagamentoRepository
{
    public ParametroPagamentoRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<ParametroPagamento?> GetAtivoAsync() =>
        DbSet.FirstOrDefaultAsync(p => p.Active);
}
