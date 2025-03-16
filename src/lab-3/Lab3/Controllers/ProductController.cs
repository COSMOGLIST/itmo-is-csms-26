using Microsoft.AspNetCore.Mvc;
using Task3.Models;
using Task3.Services;

namespace Lab3.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Creates new Product
    /// </summary>
    /// <returns>Id of a new Product</returns>
    /// <response code="200">Return Id of a new Product</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    public async Task<ActionResult<long>> CreateAsync(
        [FromBody] Product product,
        CancellationToken cancellationToken)
    {
        long id = await _productService.CreateAsync(product, cancellationToken);
        return Ok(id);
    }
}