using AEIG.Domain.Repositories;

namespace AEIG.Api.Worker;

public class CobrancaAtrasoWorker(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<CobrancaAtrasoWorker> logger) : BackgroundService
{
    private const string StatusPendente = "Pendente";
    private const string StatusEmAtraso = "Em Atraso";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var minutos = configuration.GetValue("CobrancaAtraso:IntervaloMinutos", 60);
        var intervalo = TimeSpan.FromMinutes(Math.Max(minutos, 1));
        logger.LogInformation("Varredura de cobranças vencidas a cada {Minutos} minutos.", intervalo.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var atualizadas = await MarcarVencidasAsync();
                if (atualizadas > 0)
                    logger.LogInformation("{Quantidade} cobrança(s) marcada(s) como em atraso.", atualizadas);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Falha ao marcar cobranças vencidas como em atraso.");
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

    private async Task<int> MarcarVencidasAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var status = scope.ServiceProvider.GetRequiredService<IStatusRepository>();
        var cobrancas = scope.ServiceProvider.GetRequiredService<ICobrancaRepository>();

        var pendente = await status.GetActiveByNameAsync(StatusPendente);
        var emAtraso = await status.GetActiveByNameAsync(StatusEmAtraso);
        if (pendente is null || emAtraso is null)
        {
            logger.LogWarning("Status Pendente ou Em Atraso não encontrado.");
            return 0;
        }

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var vencidas = await cobrancas.GetAllAsync(c => c.IdStatus == pendente.Id && c.DataVencimento < hoje);
        if (vencidas.Count == 0)
            return 0;

        foreach (var cobranca in vencidas)
            cobranca.IdStatus = emAtraso.Id;

        await cobrancas.UpdateRangeAsync(vencidas);
        return vencidas.Count;
    }
}
