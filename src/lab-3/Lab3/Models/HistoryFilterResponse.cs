using Task3.Models;

namespace Lab3.Models;

public record HistoryFilterResponse(IEnumerable<OrderHistory> OrderHistories);