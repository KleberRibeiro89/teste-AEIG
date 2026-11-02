using AEIG.Domain;
using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Repositories;

public class AvisoPagamentoRepository : RepositoryBase<AvisoPagamento>, IAvisoPagamentoRepository
{
    public AvisoPagamentoRepository(AEIGDbContext context) : base(context)
    {
    }

    public Task<AvisoPagamento?> GetByIdEventoAsync(string idEvento) =>
        DbSet.FirstOrDefaultAsync(a => a.Active && a.IdEvento == idEvento);

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RegistrarAsync(AvisoPagamento aviso)
    {
        try
        {
            await AddAsync(aviso);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlite && sqlite.SqliteErrorCode == 19)
        {
            Context.ChangeTracker.Clear();
            throw new AvisoDuplicadoException();
        }
    }
}
