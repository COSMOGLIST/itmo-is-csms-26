using BasicImplementation.Models;

namespace BasicImplementation.Services;

public interface IOrderHistoryService
{
    IAsyncEnumerable<OrderHistory> GetHistoryByFilterAsync(
        int cursor,
        int pageSize,
        OrderHistoryFilterQuery orderHistoryFilterQuery,
        CancellationToken cancellationToken);

    Task CreateAsync(OrderHistory orderHistory, CancellationToken cancellationToken);
}