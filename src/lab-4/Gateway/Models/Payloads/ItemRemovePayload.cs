namespace Gateway.Models.Payloads;

/// <summary>
/// Order history payload when item removed
/// </summary>
public record ItemRemovePayload(long OrderItemId) : Payload;