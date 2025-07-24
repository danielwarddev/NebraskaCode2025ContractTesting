using Microsoft.EntityFrameworkCore;

namespace ProductApi.Database;

public class ProductContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public ProductContext() { }
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
}