using BasicImplementation.Models.Payloads;

namespace BasicImplementation.Models;

public record OrderHistory(
    long OrderId,
    DateTime OrderHistoryItemCreatedAt,
    OrderHistoryItemKind OrderHistoryItemKind,
    Payload? OrderHistoryItemPayload,
    long Id = 0);