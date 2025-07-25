using System.Text.Json.Serialization;
using ProductApi.Database;

namespace ProductApi.Api.ProductDomain;

public record ProductDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("location")] string Location)
{
    public static ProductDto FromProduct(Product product) =>
        new ProductDto(product.Id, product.Name, product.Price, product.Location);
}