using Task3.Models;

namespace Task3.Services;

public interface IProductService
{
    Task<long> CreateAsync(Product product, CancellationToken cancellationToken);
}