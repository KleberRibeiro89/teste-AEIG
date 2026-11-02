namespace AEIG.Domain;

public class CobrancaStatusResumo
{
    public string NomeStatus { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorPago { get; set; }
}
