namespace Task3.Models;

public record ProductFilterQuery(
    long[] Ids,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Name = null);