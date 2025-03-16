namespace Gateway.Models;

public record HistoryFilterResponse(IEnumerable<OrderHistoryDto> OrderHistories);