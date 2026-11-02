using AEIG.Domain.Entities;
using AEIG.Domain.Repositories;

namespace AEIG.Api.Worker;

public class CobrancaPagamentoWorker(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<CobrancaPagamentoWorker> logger) : BackgroundService
{
    private const string StatusPago = "Pago";
    private const string StatusPagoComJuros = "Pago com Juros";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var minutos = configuration.GetValue("CobrancaPagamento:IntervaloMinutos", 60);
        var intervalo = TimeSpan.FromMinutes(Math.Max(minutos, 1));
        logger.LogInformation("Varredura de pagamentos a cada {Minutos} minutos.", intervalo.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var atualizadas = await MarcarPagasAsync();
                if (atualizadas > 0)
                    logger.LogInformation("{Quantidade} cobrança(s) marcada(s) como paga(s).", atualizadas);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Falha ao marcar cobranças pagas.");
            }

            try
            {
                await Task.Delay(intervalo, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task<int> MarcarPagasAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var status = scope.ServiceProvider.GetRequiredService<IStatusRepository>();
        var cobrancas = scope.ServiceProvider.GetRequiredService<ICobrancaRepository>();

        var pago = await status.GetActiveByNameAsync(StatusPago);
        var pagoComJuros = await status.GetActiveByNameAsync(StatusPagoComJuros);
        if (pago is null || pagoComJuros is null)
        {
            logger.LogWarning("Status Pago ou Pago com Juros não encontrado.");
            return 0;
        }

        var pagas = await cobrancas.GetAllAsync(c =>
            (c.DataPagamento != null || c.ValorPago > 0)
            && c.IdStatus != pago.Id
            && c.IdStatus != pagoComJuros.Id);
        if (pagas.Count == 0)
            return 0;

        foreach (var cobranca in pagas)
            cobranca.IdStatus = StatusDoPagamento(cobranca, pago, pagoComJuros).Id;

        await cobrancas.UpdateRangeAsync(pagas);
        return pagas.Count;
    }

    private static Status StatusDoPagamento(Cobranca cobranca, Status pago, Status pagoComJuros)
    {
        if (cobranca.DataPagamento is { } data && data > cobranca.DataVencimento)
            return pagoComJuros;

        if (cobranca.DataPagamento is null && cobranca.ValorPago > cobranca.Valor)
            return pagoComJuros;

        return pago;
    }
}
