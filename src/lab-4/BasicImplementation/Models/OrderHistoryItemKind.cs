namespace BasicImplementation.Models;

public enum OrderHistoryItemKind
{
    Created,
    ItemAdded,
    ItemRemoved,
    StateChanged,
    StateChangedInProcess,
}