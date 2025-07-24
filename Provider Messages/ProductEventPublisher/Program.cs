using Azure.Messaging.ServiceBus;
using ProductEventPublisher;

var publisher = new ServiceBusPublisher(new ServiceBusClient("a connection string"));
var productService = new ProductService();

var product = productService.ProcessStuff();
await publisher.PublishProduct(product);
