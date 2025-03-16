using Task3.Models;

namespace Task3.Services;

public interface IOrderService
{
    Task<long> CreateAsync(Order order, CancellationToken cancellationToken);

    Task<AddOrderItemResultType> AddOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken);

    Task RemoveOrderItemAsync(long orderId, long orderItemId, CancellationToken cancellationToken);

    Task TransferToWorkAsync(long id, CancellationToken cancellationToken);

    Task CompleteOrderAsync(long id, CancellationToken cancellationToken);

    Task CancelOrderAsync(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> GetHistoryByFilterAsync(int cursor, int pageSize, OrderHistoryFilterQuery orderHistoryFilterQuery, CancellationToken cancellationToken);
}