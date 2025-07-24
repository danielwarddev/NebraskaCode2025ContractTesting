using System.Net;
using System.Text;
using System.Text.Json;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ProductEventPublisher.ContractTests;

public record PossibleParameters(int? ProductId, string? ProductName, decimal? ProducePrice);
public record ProviderState(string Action, PossibleParameters Params, string State);

public class ProductServiceTests : IDisposable, IAsyncLifetime
{
    private readonly ProductService _productService = new();
    private readonly PactVerifier _pactVerifier;
    private readonly string _pactPath;
    private WebApplication _server;
    private readonly string _providerMockUrl = "http://localhost:26405";

    public ProductServiceTests(ITestOutputHelper outputHelper)
    {
        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput> { new XunitOutput(outputHelper), new ConsoleOutput() }
        };
        
        _pactPath = Path.Combine("Pacts", "Product Event Processor-Product Event Publisher.json");
        _pactVerifier = new PactVerifier("Product Event Publisher", config);
    }
    
    [Fact]
    public void Verify_Product_Event_Publisher_Pact_Is_Honored()
    {
        _pactVerifier
            .WithHttpEndpoint(new Uri(_providerMockUrl))
            .WithMessages(scenarios =>
            {
                scenarios.Add(
                    "A product event",
                    // This should create a business object the same way as the real app
                    () => _productService.ProcessStuff()
                );
                scenarios.Add(
                    "An product event",
                    // This should create a business object the same way as the real app
                    () => _productService.ProcessStuff()
                );
            })
            .WithFileSource(new FileInfo(_pactPath))
            .WithProviderStateUrl(new Uri($"{_providerMockUrl}/provider-states"))
            .Verify();
    }
    
    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(_providerMockUrl);
        _server = builder.Build();
        
        // Example of how to do provider states in messages
        // Also an example of how to use minimal APIs for provider states
        _server.MapPost("/provider-states", async context =>
        {
            string body;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }
            var providerState = JsonSerializer.Deserialize<ProviderState>(body)!;

            if (providerState.State == "some provider state")
            {
                // Do some setup with the state here
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(String.Empty);
        });
        
        await _server.StartAsync();
    }
    
    public void Dispose()
    {
        _pactVerifier.Dispose();
    }

    public async Task DisposeAsync()
    {
        await _server.StopAsync();
    }
}