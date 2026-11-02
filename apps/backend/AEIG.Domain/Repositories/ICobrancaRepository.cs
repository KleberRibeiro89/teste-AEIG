using AEIG.Domain.Entities;
using AEIG.Domain;

namespace AEIG.Domain.Repositories;

public interface ICobrancaRepository : IRepositoryBase<Cobranca>
{
    Task<bool> ExistsInMonthAsync(int associadoId, string referenceMonth, Guid navigationId);
    Task<bool> NossoNumeroExistsAsync(string nossoNumero, Guid navigationId);
    Task<List<CobrancaStatusResumo>> GetResumoPorStatusAsync();
}
