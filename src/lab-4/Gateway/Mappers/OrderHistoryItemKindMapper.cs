using Library;

namespace Gateway.Mappers;

public static class OrderHistoryItemKindMapper
{
    public static OrderHistoryItemKind MapToLibrary(string? orderHistoryItemKind)
    {
        return orderHistoryItemKind switch
        {
            "CREATED" => OrderHistoryItemKind.Created,
            "ITEM_ADDED" => OrderHistoryItemKind.ItemAdded,
            "ITEM_REMOVED" => OrderHistoryItemKind.ItemRemoved,
            "STATE_CHANGED" => OrderHistoryItemKind.StateChanged,
            "STATE_CHANGED_IN_PROCESS" => OrderHistoryItemKind.StateChanged,
            _ => OrderHistoryItemKind.Unspecified,
        };
    }

    public static Models.OrderHistoryItemKind MapToModel(OrderHistoryItemKind orderHistoryItemKind)
    {
        return orderHistoryItemKind switch
        {
            OrderHistoryItemKind.Created => Models.OrderHistoryItemKind.Created,
            OrderHistoryItemKind.ItemAdded => Models.OrderHistoryItemKind.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => Models.OrderHistoryItemKind.ItemRemoved,
            OrderHistoryItemKind.StateChanged => Models.OrderHistoryItemKind.StateChanged,
            OrderHistoryItemKind.StateChangedInProcess => Models.OrderHistoryItemKind.StateChangedInProcess,
            OrderHistoryItemKind.Unspecified => throw new Exception("Unknown order history item kind"),
            _ => throw new Exception("Unknown order history item kind"),
        };
    }
}