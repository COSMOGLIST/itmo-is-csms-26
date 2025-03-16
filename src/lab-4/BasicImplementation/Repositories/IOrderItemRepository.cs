using BasicImplementation.Models;

namespace BasicImplementation.Repositories;

public interface IOrderItemRepository
{
    Task<long> CreateAsync(OrderItem orderItem, CancellationToken cancellationToken);

    Task UpdateAsync(OrderItem orderItem, CancellationToken cancellationToken);

    Task<OrderItem> GetByIdAsync(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> GetByFilterAsync(int cursor, int pageSize, OrderItemsFilterQuery orderItemsFilterQuery, CancellationToken cancellationToken);
}