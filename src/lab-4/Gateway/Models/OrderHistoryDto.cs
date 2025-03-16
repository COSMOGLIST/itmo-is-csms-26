using Gateway.Models.Payloads;

namespace Gateway.Models;

public record OrderHistoryDto(
    long Id,
    long OrderId,
    DateTime OrderHistoryItemCreatedAt,
    OrderHistoryItemKind OrderHistoryItemKind,
    Payload? OrderHistoryItemPayload);