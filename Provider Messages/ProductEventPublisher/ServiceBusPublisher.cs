using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace ProductEventPublisher;

public class ServiceBusPublisher
{
    private ServiceBusSender _sender;
    
    public ServiceBusPublisher(ServiceBusClient client)
    {
        _sender = client.CreateSender("product-queue");
    }

    public async Task PublishProduct(Product product)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(product))
        {
            ContentType = "application/json",
        };
        await _sender.SendMessageAsync(message);
    }
}
