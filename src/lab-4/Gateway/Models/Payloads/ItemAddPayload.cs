namespace Gateway.Models.Payloads;

/// <summary>
/// Order history payload when item added
/// </summary>
public record ItemAddPayload(long OrderItemId) : Payload;