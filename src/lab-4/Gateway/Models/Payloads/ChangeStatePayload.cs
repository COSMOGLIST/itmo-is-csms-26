namespace Gateway.Models.Payloads;

/// <summary>
/// Order history payload when state changed
/// </summary>
public record ChangeStatePayload(OrderState PreviousState, OrderState NewState) : Payload;