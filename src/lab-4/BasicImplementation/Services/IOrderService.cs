using BasicImplementation.Models;

namespace BasicImplementation.Services;

public interface IOrderService
{
    Task<long> CreateAsync(Order order, CancellationToken cancellationToken);

    Task<AddOrderItemResultType> AddOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken);

    Task RemoveOrderItemAsync(long orderId, long orderItemId, CancellationToken cancellationToken);

    Task TransferToWorkAsync(long id, CancellationToken cancellationToken);

    Task CompleteOrderAsync(long id, CancellationToken cancellationToken);

    Task CancelOrderAsync(long id, CancellationToken cancellationToken);

    Task CancelOrderPrivilegedAsync(long id, CancellationToken cancellationToken);
}