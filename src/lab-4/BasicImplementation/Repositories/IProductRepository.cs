using BasicImplementation.Models;

namespace BasicImplementation.Repositories;

public interface IProductRepository
{
    Task<long> CreateAsync(Product product, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> GetByFilterAsync(int pageToken, int pageSize, ProductFilterQuery productFilterQuery, CancellationToken cancellationToken);
}