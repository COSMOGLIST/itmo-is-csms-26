namespace Task3.Models;

public record OrderItemsFilterQuery(
    long[] OrderIds,
    long[] ProductIds,
    bool? Deleted = null);