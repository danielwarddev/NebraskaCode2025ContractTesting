using System.Net;
using AwesomeAssertions;
using PactNet;

namespace ProductProcessor.IntegrationTests;

public class ProductClientTests
{
    private readonly HttpClient _httpClient;
    private readonly ProductClient _productClient;
    private readonly IPactBuilderV4 _pactBuilder = Pact
        .V4("Product Processor", "Product API", new PactConfig())
        .WithHttpInteractions();

    public ProductClientTests()
    {
        _httpClient = new HttpClient();
        _httpClient.AddRequestHeaders(ProductClient.RequestHeaders);
        _productClient = new ProductClient(_httpClient);
    }
    
    // Out of the box Pact
    [Fact]
    public async Task Api_Returns_All_Products()
    {
        var expectedProducts = new [] { new Product(1, "A cool product", 10.50m, "Cool Store #12345") };
        
        _pactBuilder
            .UponReceiving("A GET request to retrieve all products")
            .Given("Products exist")
            .WithRequest(HttpMethod.Get, "/products")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(expectedProducts);
        
        await _pactBuilder.VerifyAsync(async context =>
        {
            _httpClient.BaseAddress = context.MockServerUri;
            var actualProducts = await _productClient.GetAllProducts();
            actualProducts.Should().BeEquivalentTo(expectedProducts);
        });
    }
    
    // Cleaned up a little by abstracting headers
    [Fact]
    public async Task Api_Returns_All_Products_With_Name()
    {
        var expectedProducts = new [] { new Product(1, "A cool product", 10.50m, "Cool Store #12345") };
        
        _pactBuilder
            .UponReceiving("A GET request to retrieve all products with given id and name")
            .Given("A product exists")
            .WithRequest(HttpMethod.Get, "/products")
            .WithQuery("name", "A cool product")
            .WithHeaders(ProductClient.RequestHeaders)
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(expectedProducts);

        /*_pactBuilder
            .UponReceivingGet(
                "A GET request to retrieve all products with given id and name",
                "A product exists",
                "/products")
            .WillRespondOk(expectedProducts);*/
        
        await _pactBuilder.VerifyAsync(async context =>
        {
            _httpClient.BaseAddress = context.MockServerUri;
            var actualProducts = await _productClient.GetProductsByName("A cool product");
            actualProducts.Should().BeEquivalentTo(expectedProducts);
        });
    }
    
    // Cleaned up a little more by abstracting common request + response values
    [Fact]
    public async Task When_Product_Exists_Then_Api_Returns_Product()
    {
        var expectedProduct = new Product(1, "Cool product", 10.50m, "Cool Store #12345");

        _pactBuilder
            .UponReceivingGet(
                "/product/1",
                "A GET request to retrieve a product",
                "A product with id 1 exists",
                new Dictionary<string, string>
                {
                    { "productId", "1" },
                    { "productName", "Cool product" }
                })
            .WillRespondOk(expectedProduct);
        
        await _pactBuilder.VerifyAsync(async context =>
        {
            _httpClient.BaseAddress = context.MockServerUri;
            var actualProduct = await _productClient.GetProduct(1);
            actualProduct.Should().BeEquivalentTo(expectedProduct);
        });
    }
    
    [Fact]
    public async Task When_Product_Does_Not_Exist_Then_Api_Returns_404()
    {
        _pactBuilder
            .UponReceiving("A GET request to retrieve a product")
            .Given("There is not a product with id 1")
            .WithRequest(HttpMethod.Get, "/product/1")
            .WithHeaders(ProductClient.RequestHeaders)
            .WillRespond()
            .WithStatus(HttpStatusCode.NotFound);
        
        await _pactBuilder.VerifyAsync(async context =>
        {
            _httpClient.BaseAddress = context.MockServerUri;
            var actualProduct = await _productClient.GetProduct(1);
            actualProduct.Should().BeNull();
        });
    }
}
