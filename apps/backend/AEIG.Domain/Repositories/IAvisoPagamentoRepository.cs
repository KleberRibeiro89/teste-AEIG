using AEIG.Domain.Entities;

namespace AEIG.Domain.Repositories;

public interface IAvisoPagamentoRepository : IRepositoryBase<AvisoPagamento>
{
    Task<AvisoPagamento?> GetByIdEventoAsync(string idEvento);
    Task ExecuteInTransactionAsync(Func<Task> action);
    Task RegistrarAsync(AvisoPagamento aviso);
}
