namespace AEIG.Domain.AppService.Extensions;

public static class DecimalExtensions
{
    public static decimal RoundMoney(this decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
