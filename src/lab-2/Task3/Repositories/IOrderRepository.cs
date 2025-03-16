using Task3.Models;

namespace Task3.Repositories;

public interface IOrderRepository
{
    Task<long> CreateAsync(Order order, CancellationToken cancellationToken);

    Task UpdateAsync(Order order, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> GetByFilterAsync(int cursor, int pageSize, OrderFilterQuery orderFilterQuery, CancellationToken cancellationToken);

    Task<Order> GetByIdAsync(long id, CancellationToken cancellationToken);
}