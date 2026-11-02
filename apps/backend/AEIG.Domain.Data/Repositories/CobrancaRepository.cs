using AEIG.Domain;
using AEIG.Domain.Data.Extensions;
using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class CobrancaRepository : RepositoryBase<Cobranca>, ICobrancaRepository
{
    public CobrancaRepository(AEIGDbContext context) : base(context)
    {
    }

    public override Task<AEIG.Domain.DynamicQueryResult<Cobranca>> GetPagedAsync(AEIG.Domain.DynamicQuery query) =>
        DbSet.Where(c => c.Active).Include(c => c.Status).ToPagedAsync(query);

    public Task<bool> ExistsInMonthAsync(int associadoId, string referenceMonth, Guid navigationId) =>
        DbSet.AnyAsync(c =>
            c.Active
            && c.IdAssociado == associadoId
            && c.MesReferencia == referenceMonth
            && c.NavigationId != navigationId);

    public Task<bool> NossoNumeroExistsAsync(string nossoNumero, Guid navigationId) =>
        DbSet.AnyAsync(c => c.NossoNumero == nossoNumero && c.NavigationId != navigationId);

    public Task<Cobranca?> GetActiveByNossoNumeroAsync(string nossoNumero) =>
        DbSet.Include(c => c.Status).FirstOrDefaultAsync(c => c.Active && c.NossoNumero == nossoNumero);

    public Task<List<CobrancaStatusResumo>> GetResumoPorStatusAsync() =>
        DbSet.Where(c => c.Active)
            .GroupBy(c => c.Status!.Nome)
            .Select(g => new CobrancaStatusResumo
            {
                NomeStatus = g.Key,
                Quantidade = g.Count(),
                Valor = g.Sum(c => c.Valor),
                ValorPago = g.Sum(c => c.ValorPago)
            })
            .ToListAsync();

    public Task<List<CobrancaMesResumo>> GetRecebidosPorMesAsync() =>
        DbSet.Where(c => c.Active && (c.Status!.Nome == "Pago" || c.Status.Nome == "Pago com Juros"))
            .GroupBy(c => c.MesReferencia)
            .Select(g => new CobrancaMesResumo
            {
                Mes = g.Key,
                Valor = g.Sum(c => c.ValorPago)
            })
            .ToListAsync();

    public Task<List<CobrancaMesResumo>> GetAtrasosPorMesAsync() =>
        DbSet.Where(c => c.Active && c.Status!.Nome == "Em Atraso")
            .GroupBy(c => c.MesReferencia)
            .Select(g => new CobrancaMesResumo
            {
                Mes = g.Key,
                Valor = g.Sum(c => c.Valor)
            })
            .ToListAsync();
}
