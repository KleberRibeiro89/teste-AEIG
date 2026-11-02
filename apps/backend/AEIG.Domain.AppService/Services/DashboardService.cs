using AEIG.Domain.AppService.Dtos.Responses.DashboardResponses;
using AEIG.Domain.AppService.Interfaces;
using AEIG.Domain.Repositories;

namespace AEIG.Domain.AppService.Services;

public class DashboardService : IDashboardService
{
    private static readonly string[] OrdemStatus = ["Pendente", "Em Atraso", "Pago", "Pago com Juros"];

    private readonly IAssociadoRepository _associadoRepository;
    private readonly ICobrancaRepository _cobrancaRepository;

    public DashboardService(IAssociadoRepository associadoRepository, ICobrancaRepository cobrancaRepository)
    {
        _associadoRepository = associadoRepository;
        _cobrancaRepository = cobrancaRepository;
    }

    public async Task<DashboardResponse> GetAsync()
    {
        var associados = await _associadoRepository.CountActiveAsync();
        var resumo = await _cobrancaRepository.GetResumoPorStatusAsync();
        var porNome = resumo.ToDictionary(item => item.NomeStatus, StringComparer.OrdinalIgnoreCase);

        var cobrancas = OrdemStatus.Select(nome =>
        {
            porNome.TryGetValue(nome, out var item);
            return new DashboardStatusResponse
            {
                Nome = nome,
                Quantidade = item?.Quantidade ?? 0,
                Valor = item?.Valor ?? 0,
                ValorPago = item?.ValorPago ?? 0
            };
        }).ToList();

        foreach (var extra in resumo.Where(item => !OrdemStatus.Contains(item.NomeStatus, StringComparer.OrdinalIgnoreCase)))
        {
            cobrancas.Add(new DashboardStatusResponse
            {
                Nome = extra.NomeStatus,
                Quantidade = extra.Quantidade,
                Valor = extra.Valor,
                ValorPago = extra.ValorPago
            });
        }

        return new DashboardResponse
        {
            Associados = associados,
            Cobrancas = cobrancas
        };
    }
}
