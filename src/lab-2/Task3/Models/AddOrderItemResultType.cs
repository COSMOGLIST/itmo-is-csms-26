namespace Task3.Models;

public record AddOrderItemResultType
{
    private AddOrderItemResultType() { }

    public sealed record Success(long OrderItemId) : AddOrderItemResultType;

    public sealed record OrderIsNotCreatedType : AddOrderItemResultType;
}