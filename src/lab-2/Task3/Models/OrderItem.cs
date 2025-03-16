namespace Task3.Models;

public record OrderItem(
    long OrderId,
    long ProductId,
    int OrderItemQuantity,
    bool OrderItemDeleted = false,
    long Id = 0);