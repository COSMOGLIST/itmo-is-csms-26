using BasicImplementation.Models;

namespace BasicImplementation.Repositories;

public interface IOrderHistoryRepository
{
    Task<long> CreateAsync(OrderHistory orderHistory, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> GetByFilterAsync(int cursor, int pageSize, OrderHistoryFilterQuery orderHistoryFilterQuery, CancellationToken cancellationToken);
}