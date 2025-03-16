using BasicImplementation.Models;
using BasicImplementation.Services;
using Grpc.Core;
using Library;
using OrderService.Mappers;

namespace OrderService.Controllers;

public class ProductController : Library.ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public override async Task<CreateProductReply> Create(CreateProductRequest request, ServerCallContext context)
    {
        var order = new Product(request.ProductName, MoneyMapper.Map(request.Money));
        var createReply = new CreateProductReply
        {
            Id = await _productService.CreateAsync(order, context.CancellationToken),
        };
        return createReply;
    }
}