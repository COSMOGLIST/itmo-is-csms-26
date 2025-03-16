namespace Task3.Models.Payloads;

public record ItemRemovePayload(long OrderItemId) : Payloads.Payload;