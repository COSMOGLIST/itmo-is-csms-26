using BasicImplementation.Models;

namespace BasicImplementation.Services;

public interface IProductService
{
    Task<long> CreateAsync(Product product, CancellationToken cancellationToken);
}