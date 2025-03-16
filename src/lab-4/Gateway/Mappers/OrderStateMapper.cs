using Library;

namespace Gateway.Mappers;

public static class OrderStateMapper
{
    public static Models.OrderState Map(OrderState orderState)
    {
        return orderState switch
        {
            OrderState.Created => Models.OrderState.Created,
            OrderState.Cancelled => Models.OrderState.Cancelled,
            OrderState.Completed => Models.OrderState.Completed,
            OrderState.Processing => Models.OrderState.Processing,
            OrderState.Unspecified => throw new Exception("Unknown order state"),
            _ => throw new Exception("Unknown order state"),
        };
    }
}