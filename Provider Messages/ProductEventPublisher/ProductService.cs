namespace ProductEventPublisher;

public record Product(int Id, string Name, decimal Price, string Location);

public interface IProductService
{
    Product ProcessStuff();
}
public class ProductService : IProductService
{
    // Does stuff that ends up with a product
    public Product ProcessStuff()
    {
        return new Product(1, "Pet Rock", 20, "Des Moines");
    }
}