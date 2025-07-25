using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ProductApi.ContractTests;

public class ProductControllerTests : IClassFixture<ProductApiFixture>
{
    private readonly ProductApiFixture _apiFixture;
    private readonly ITestOutputHelper _outputHelper;
    
    public ProductControllerTests(ProductApiFixture apiFixture, ITestOutputHelper outputHelper)
    {
        _apiFixture = apiFixture;
        _outputHelper = outputHelper;
    }
    
    [Theory]
    [InlineData("Product Processor-Product API.json")]
    [InlineData("Product Processor TS-Product API.json")]
    public void Verify_Contract_Is_Honored(string contractFileName)
    {
        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput> { new XunitOutput(_outputHelper), new ConsoleOutput() }
        };

        var pactPath = Path.Combine("Pacts", contractFileName);
        var pactVerifier = new PactVerifier("Product API", config);
        
        pactVerifier
            .WithHttpEndpoint(_apiFixture.PactServerUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(_apiFixture.PactServerUri, "/provider-states"))
            .Verify();
    }
}