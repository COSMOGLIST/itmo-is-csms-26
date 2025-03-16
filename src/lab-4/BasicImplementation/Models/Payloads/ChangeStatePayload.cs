namespace BasicImplementation.Models.Payloads;

public record ChangeStatePayload(OrderState PreviousState, OrderState NewState) : Payload;