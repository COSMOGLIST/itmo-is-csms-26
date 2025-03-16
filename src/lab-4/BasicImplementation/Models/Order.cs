namespace BasicImplementation.Models;

public record Order(
    DateTime OrderCreatedAt,
    string OrderCreatedBy,
    OrderState OrderState = OrderState.Created,
    long Id = 0);