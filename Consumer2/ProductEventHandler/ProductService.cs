namespace ProductEventHandler;

public interface IProductService
{
    Task ProcessProduct(Product product);
}

public class ProductService : IProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task ProcessProduct(Product product)
    {
        // Do something with the product after pulling it from the bus
        await _productRepository.InsertProduct(product);
    }
}