using AEIG.Domain.Entities;

namespace AEIG.Domain.Repositories;

public interface IParametroPagamentoRepository : IRepositoryBase<ParametroPagamento>
{
    Task<ParametroPagamento?> GetAtivoAsync();
}
