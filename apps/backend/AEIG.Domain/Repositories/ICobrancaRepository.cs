using AEIG.Domain.Entities;
using AEIG.Domain;

namespace AEIG.Domain.Repositories;

public interface ICobrancaRepository : IRepositoryBase<Cobranca>
{
    Task<bool> ExistsInMonthAsync(int associadoId, string referenceMonth, Guid navigationId);
    Task<bool> NossoNumeroExistsAsync(string nossoNumero, Guid navigationId);
    Task<Cobranca?> GetActiveByNossoNumeroAsync(string nossoNumero);
    Task<List<CobrancaStatusResumo>> GetResumoPorStatusAsync();
    Task<List<CobrancaMesResumo>> GetRecebidosPorMesAsync();
    Task<List<CobrancaMesResumo>> GetAtrasosPorMesAsync();
}
