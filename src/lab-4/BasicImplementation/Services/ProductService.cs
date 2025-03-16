using BasicImplementation.Models;
using BasicImplementation.Repositories;
using System.Transactions;

namespace BasicImplementation.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<long> CreateAsync(Product product, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        long id = await _productRepository.CreateAsync(product, cancellationToken);
        transaction.Complete();
        return id;
    }
}