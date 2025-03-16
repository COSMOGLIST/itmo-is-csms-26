using Task3.Models.Payloads;

namespace Task3.Models;

public record OrderHistory(
    long OrderId,
    DateTime OrderHistoryItemCreatedAt,
    OrderHistoryItemKind OrderHistoryItemKind,
    Payload? OrderHistoryItemPayload,
    long Id = 0);