using Gateway.Mappers;
using Gateway.Models;
using Library;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly ProductService.ProductServiceClient _client;

    public ProductController(ProductService.ProductServiceClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Creates new Product
    /// </summary>
    /// <returns>Id of a new Product</returns>
    /// <response code="200">Return Id of a new Product</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    public async Task<ActionResult<long>> CreateAsync(
        [FromBody] ProductDto product,
        CancellationToken cancellationToken)
    {
        CreateProductReply createProductReply = await _client.CreateAsync(
            new CreateProductRequest
            {
                ProductName = product.ProductName,
                Money = MoneyMapper.Map(product.ProductPrice),
            },
            cancellationToken: cancellationToken);
        return Ok(createProductReply.Id);
    }
}