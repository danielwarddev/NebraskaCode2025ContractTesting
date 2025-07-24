using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProductApi.Api.ProductDomain;
using ProductApi.Database;

namespace ProductApi.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddScoped<IProductService, ProductService>();
        services.AddDbContext<ProductContext>(options => options.UseNpgsql(
            "Host=localhost;Database=products;Username=postgres;Password=postgres"
        ));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseRouting().UseEndpoints(endpoints => endpoints.MapOpenApi());
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRouting().UseEndpoints(endpoints => endpoints.MapControllers());
    }
}