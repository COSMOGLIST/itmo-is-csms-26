using Library;

namespace OrderService.Mappers;

public static class MoneyMapper
{
    public static decimal Map(Money money)
    {
        return money.Units + (money.Nanos / 1_000_000_000m);
    }
}