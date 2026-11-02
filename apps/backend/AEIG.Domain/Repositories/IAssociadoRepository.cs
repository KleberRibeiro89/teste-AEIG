using AEIG.Domain.Entities;

namespace AEIG.Domain.Repositories;

public interface IAssociadoRepository : IRepositoryBase<Associado>
{
    Task<Associado?> GetByCpfAsync(string cpf);
}
