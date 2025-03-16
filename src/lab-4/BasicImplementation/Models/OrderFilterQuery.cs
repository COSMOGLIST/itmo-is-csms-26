namespace BasicImplementation.Models;

public record OrderFilterQuery(
    long[] Ids,
    OrderState? OrderState = null,
    string? Author = null);