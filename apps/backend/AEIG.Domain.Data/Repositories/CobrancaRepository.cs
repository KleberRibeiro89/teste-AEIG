using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class CobrancaRepository : RepositoryBase<Cobranca>, ICobrancaRepository
{
    public CobrancaRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<bool> ExistsInMonthAsync(int associadoId, string referenceMonth, Guid navigationId) =>
        DbSet.AnyAsync(c =>
            c.Active
            && c.IdAssociado == associadoId
            && c.MesReferencia == referenceMonth
            && c.NavigationId != navigationId);

    public Task<bool> NossoNumeroExistsAsync(string nossoNumero, Guid navigationId) =>
        DbSet.AnyAsync(c => c.NossoNumero == nossoNumero && c.NavigationId != navigationId);
}
