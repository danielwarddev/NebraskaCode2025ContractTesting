namespace ProductEventHandler;

public interface IProductService
{
    Task ProcessProduct(Product product);
}

public class ProductService : IProductService
{
    public async Task ProcessProduct(Product product)
    {
        // Do something with the product after pulling it from the bus
        await Task.CompletedTask;
    }
}