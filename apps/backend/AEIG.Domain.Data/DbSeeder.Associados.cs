using AEIG.Domain.Entities;
using AEIG.Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data;

public static partial class DbSeeder
{
    private const decimal MultaPercentual = 2m;
    private const decimal JurosDiarioPercentual = 0.033m;

    public static async Task SeedAssociadosAsync(AEIGDbContext context)
    {
        if (await context.Set<Associado>().AnyAsync(a => a.Active))
            return;

        var status = await context.Set<Status>().Where(s => s.Active).ToDictionaryAsync(s => s.Nome);
        var pendente = status["Pendente"];
        var emAtraso = status["Em Atraso"];
        var pago = status["Pago"];
        var pagoComJuros = status["Pago com Juros"];

        var agora = DateTime.UtcNow;
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var amostras = Amostras();
        var associados = amostras.Select(amostra => new Associado
        {
            NavigationId = Guid.NewGuid(),
            Nome = amostra.Nome,
            Cpf = amostra.Cpf,
            ValorMensalidade = amostra.Mensalidade,
            DiaVencimento = amostra.DiaVencimento,
            CreatedAt = agora,
            CreatedBy = Guid.Empty,
            Active = true
        }).ToList();

        await context.Set<Associado>().AddRangeAsync(associados);
        await context.SaveChangesAsync();

        var cobrancas = new List<Cobranca>();
        var primeiroMes = new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-10);
        for (var index = 0; index < associados.Count; index++)
        {
            var associado = associados[index];
            for (var offset = 0; offset < 12; offset++)
            {
                var mes = primeiroMes.AddMonths(offset);
                var dia = Math.Clamp(associado.DiaVencimento, 1, DateTime.DaysInMonth(mes.Year, mes.Month));
                var vencimento = new DateOnly(mes.Year, mes.Month, dia);
                var (idStatus, pagamento, valorPago) = Situacao(
                    index,
                    offset,
                    vencimento,
                    hoje,
                    associado.ValorMensalidade,
                    pendente.Id,
                    emAtraso.Id,
                    pago.Id,
                    pagoComJuros.Id);

                cobrancas.Add(new Cobranca
                {
                    NavigationId = Guid.NewGuid(),
                    IdAssociado = associado.Id,
                    NossoNumero = $"{index + 1:D4}{vencimento:yyyyMMdd}{offset:D2}{idStatus:D2}0000",
                    MesReferencia = $"{mes:yyyy-MM}",
                    Valor = associado.ValorMensalidade,
                    ValorPago = valorPago,
                    DataVencimento = vencimento,
                    DataPagamento = pagamento,
                    IdStatus = idStatus,
                    CreatedAt = agora,
                    CreatedBy = Guid.Empty,
                    Active = true
                });
            }
        }

        await context.Set<Cobranca>().AddRangeAsync(cobrancas);
        await context.SaveChangesAsync();
    }

    private static (int IdStatus, DateOnly? Pagamento, decimal ValorPago) Situacao(
        int associado,
        int mes,
        DateOnly vencimento,
        DateOnly hoje,
        decimal valor,
        int pendente,
        int emAtraso,
        int pago,
        int pagoComJuros)
    {
        if (vencimento >= hoje)
            return (pendente, null, 0);

        var perfil = associado % 4;
        var diasAtraso = 8 + (associado % 5) * 6;
        var pagamentoAtrasado = vencimento.AddDays(diasAtraso);
        if (pagamentoAtrasado > hoje)
            pagamentoAtrasado = hoje;

        if (perfil == 0)
            return (pago, vencimento, valor);

        if (perfil == 1)
            return mes % 2 == 0
                ? (pagoComJuros, pagamentoAtrasado, ValorComJuros(valor, vencimento, pagamentoAtrasado))
                : (pago, vencimento, valor);

        if (perfil == 2)
            return mes < 6
                ? (emAtraso, null, 0)
                : (pago, vencimento, valor);

        return mes == 0
            ? (pagoComJuros, pagamentoAtrasado, ValorComJuros(valor, vencimento, pagamentoAtrasado))
            : (emAtraso, null, 0);
    }

    private static decimal ValorComJuros(decimal valor, DateOnly vencimento, DateOnly pagamento)
    {
        var dias = Math.Max(0, pagamento.DayNumber - vencimento.DayNumber);
        var multa = RoundMoney(valor * MultaPercentual / 100m);
        var juros = RoundMoney(valor * JurosDiarioPercentual / 100m * dias);
        return RoundMoney(valor + multa + juros);
    }

    private static decimal RoundMoney(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    private static (string Nome, string Cpf, decimal Mensalidade, int DiaVencimento)[] Amostras()
    {
        var nomes = new[]
        {
            "Ana Souza", "Bruno Lima", "Carla Mendes", "Diego Alves", "Elena Costa",
            "Fabio Rocha", "Gabriela Nunes", "Henrique Dias", "Irene Campos", "João Pereira",
            "Karina Lopes", "Lucas Martins", "Marina Teixeira", "Nelson Duarte", "Olivia Ramos",
            "Paulo Ferreira", "Queila Barbosa", "Rafael Monteiro", "Sabrina Castro", "Tiago Moreira"
        };
        var mensalidades = new decimal[]
        {
            90, 120, 150, 180, 220, 260, 300, 350, 420, 480,
            550, 640, 750, 890, 980, 1200, 1500, 1800, 2200, 2500
        };
        var dias = new[] { 1, 2, 5, 8, 10, 12, 15, 18, 20, 22, 25, 28, 3, 7, 14, 16, 21, 24, 27, 30 };

        return nomes.Select((nome, index) => (nome, GerarCpf(index + 1), mensalidades[index], dias[index])).ToArray();
    }

    private static string GerarCpf(int ordem)
    {
        var numbers = new int[11];
        var value = 235711131 + ordem * 104729;
        for (var i = 8; i >= 0; i--)
        {
            numbers[i] = value % 10;
            value /= 10;
        }

        if (numbers.Take(9).Distinct().Count() == 1)
            numbers[8] = (numbers[8] + 1) % 10;

        numbers[9] = DigitoCpf(numbers, 9);
        numbers[10] = DigitoCpf(numbers, 10);
        var cpf = string.Concat(numbers);
        if (!CpfValidator.IsValid(cpf))
            throw new InvalidOperationException($"CPF de seed inválido: {cpf}");
        return cpf;
    }

    private static int DigitoCpf(int[] numbers, int length)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
            sum += numbers[i] * (length + 1 - i);

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
