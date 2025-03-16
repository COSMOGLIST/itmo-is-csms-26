namespace Gateway.Models;

public enum OrderHistoryItemKind
{
    Created,
    ItemAdded,
    ItemRemoved,
    StateChanged,
    StateChangedInProcess,
}