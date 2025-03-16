using Library;

namespace OrderService.Mappers;

public static class OrderStateMapper
{
    public static OrderState Map(BasicImplementation.Models.OrderState orderState)
    {
        return orderState switch
        {
            BasicImplementation.Models.OrderState.Created => OrderState.Created,
            BasicImplementation.Models.OrderState.Cancelled => OrderState.Cancelled,
            BasicImplementation.Models.OrderState.Completed => OrderState.Completed,
            BasicImplementation.Models.OrderState.Processing => OrderState.Processing,
            _ => OrderState.Unspecified,
        };
    }
}