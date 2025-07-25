using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace ProductEventHandler;

public class ProductService_Old
{
    [FunctionName("ProductListener")]
    public async Task Run(
        [ServiceBusTrigger("product-queue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var bodyString = message.Body.ToString();
        var product = JsonSerializer.Deserialize<Product>(bodyString)!;

        await ProcessProduct(product);
    }
    
    private Task ProcessProduct(Product product)
    {
        return Task.CompletedTask;
    }
}