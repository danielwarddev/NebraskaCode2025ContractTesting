using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace ProductEventHandler;

public record Product(int Id, string Name, decimal Price, string Location);

public class ServiceBusListener
{
    private readonly IProductService _productService;

    public ServiceBusListener(IProductService productService)
    {
        _productService = productService;
    }
    
    [FunctionName("ProductListener")]
    public async Task Run(
        [ServiceBusTrigger("product-queue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var bodyString = message.Body.ToString();
        var product = JsonSerializer.Deserialize<Product>(bodyString)!;

        await _productService.ProcessProduct(product);
        
        await Task.CompletedTask;
    }
}