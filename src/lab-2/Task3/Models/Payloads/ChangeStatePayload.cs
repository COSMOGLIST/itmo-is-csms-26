namespace Task3.Models.Payloads;

public record ChangeStatePayload(OrderState PreviousState, OrderState NewState) : Payload;