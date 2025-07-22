using Microsoft.AspNetCore.Mvc;
using ProductApi.Api.ProductDomain;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("/products")]
    public async Task<ActionResult<ProductDto>> GetAllProducts([FromQuery] string? name = null)
    {
        if (name != null)
        {
            var products = await _productService.GetProductsByName(name);
            return Ok(products.Select(ProductDto.FromProduct));
        }

        var product = await _productService.GetAllProducts();
        return Ok(product.Select(ProductDto.FromProduct));
    }

    [HttpGet("/product/{productId:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int productId)
    {
        var product = await _productService.GetProduct(productId);
        if (product == null)
        {
            return NotFound();
        }
        
        return Ok(ProductDto.FromProduct(product));
    }
}