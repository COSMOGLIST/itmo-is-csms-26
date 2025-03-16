namespace Gateway.Models;

public record OrderDto(
    DateTime OrderCreatedAt,
    string OrderCreatedBy);