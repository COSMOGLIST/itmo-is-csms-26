namespace BasicImplementation.Models;

public record OrderHistoryFilterQuery(
    long? OrderId = null,
    OrderHistoryItemKind? OrderHistoryItemKind = null);