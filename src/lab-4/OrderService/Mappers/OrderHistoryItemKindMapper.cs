using Library;

namespace OrderService.Mappers;

public static class OrderHistoryItemKindMapper
{
    public static OrderHistoryItemKind Map(BasicImplementation.Models.OrderHistoryItemKind orderHistoryItemKind)
    {
        return orderHistoryItemKind switch
        {
            BasicImplementation.Models.OrderHistoryItemKind.Created => OrderHistoryItemKind.Created,
            BasicImplementation.Models.OrderHistoryItemKind.ItemAdded => OrderHistoryItemKind.ItemAdded,
            BasicImplementation.Models.OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKind.ItemRemoved,
            BasicImplementation.Models.OrderHistoryItemKind.StateChanged => OrderHistoryItemKind.StateChanged,
            BasicImplementation.Models.OrderHistoryItemKind.StateChangedInProcess => OrderHistoryItemKind.StateChangedInProcess,
            _ => OrderHistoryItemKind.Unspecified,
        };
    }

    public static BasicImplementation.Models.OrderHistoryItemKind? Map(OrderHistoryItemKind orderHistoryItemKind)
    {
        return orderHistoryItemKind switch
        {
            OrderHistoryItemKind.Created => BasicImplementation.Models.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => BasicImplementation.Models.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => BasicImplementation.Models.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => BasicImplementation.Models.OrderHistoryItemKind.StateChanged,
            OrderHistoryItemKind.StateChangedInProcess => BasicImplementation.Models.OrderHistoryItemKind.StateChangedInProcess,
            OrderHistoryItemKind.Unspecified => null,
            _ => throw new Exception("Unknown order history item kind"),
        };
    }
}