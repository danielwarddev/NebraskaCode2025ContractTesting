using PactNet;
using Match = PactNet.Matchers.Match;

namespace ProductEventHandler.IntegrationTests;

public class ProductServiceTests
{
private readonly ProductService _productService = new();
    private readonly IMessagePactBuilderV4 _pactBuilder = Pact
        .V4("Product Event Handler", "Product Event Publisher", new PactConfig())
        .WithMessageInteractions();
 
    [Fact]
    public async Task Inserts_Product_Into_Database()
    {
        var expectedProductEvent = new Product(1, "Rubber Chicken Deluxe", 50, "San Antonio");

        await _pactBuilder
            .ExpectsToReceive("A product event")
            .WithMetadata("contentType", "application/json")
            .WithJsonContent(Match.Type(expectedProductEvent))
            .VerifyAsync<Product>(async productEvent =>
            {
                await _productService.ProcessProduct(productEvent);
                
                // In a real app, you'd also have an assertion here
                // eg. checking if the product got inserted into the database for a Created event
                // For simplicity's sake, this is not shown here
            });
    }

    [Fact]
    public async Task Inserts_Product_Into_Database_With_State()
    {
        var expectedProductEvent = new Product(1, "10lb Frozen Burrito", 10, "Nebraska");

        await _pactBuilder
            .ExpectsToReceive("An product event")
            // Given() is probably more rarely needed for message contracts
            // You're not expecting multiple status codes, it either goes on the queue or it doesn't
            .Given("some expected provider state")
            .WithMetadata("contentType", "application/json")
            .WithJsonContent(Match.Type(expectedProductEvent))
            .VerifyAsync<Product>(async productEvent =>
            {
                await _productService.ProcessProduct(productEvent);
                // Some assert statement
            });
    }
}