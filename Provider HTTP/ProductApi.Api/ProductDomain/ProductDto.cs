using System.Text.Json.Serialization;
using ProductApi.Database;

namespace ProductApi.Api.ProductDomain;

public record ProductDto(
    [property: JsonPropertyName("Id")] int Id,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Price")] decimal Price,
    [property: JsonPropertyName("Location")] string Location)
{
    public static ProductDto FromProduct(Product product) =>
        new ProductDto(product.Id, product.Name, product.Price, product.Location);
}