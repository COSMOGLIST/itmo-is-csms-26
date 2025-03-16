using Library;

namespace Gateway.Mappers;

public static class MoneyMapper
{
    public static Money Map(decimal amount)
    {
        long units = Convert.ToInt64(Math.Truncate(amount));
        decimal fractionalPart = amount - units;
        int nanos = Convert.ToInt32(Math.Round(fractionalPart * 1_000_000_000));
        return new Money
        {
            Units = units,
            Nanos = nanos,
        };
    }
}