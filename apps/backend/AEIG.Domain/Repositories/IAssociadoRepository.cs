using AEIG.Domain.Entities;

namespace AEIG.Domain.Repositories;

public interface IAssociadoRepository : IRepositoryBase<Associado>
{
    Task<Associado?> GetByCpfAsync(string cpf);
    Task<Associado?> GetActiveByIdAsync(int id);
    Task<bool> ExistsActiveAsync(int id);
    Task<int> CountActiveAsync();
}
