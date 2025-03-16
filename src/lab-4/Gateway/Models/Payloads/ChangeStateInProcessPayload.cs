namespace Gateway.Models.Payloads;

public record ChangeStateInProcessPayload(string OrderProcessingEvent, bool IsSuccessful) : Payload;