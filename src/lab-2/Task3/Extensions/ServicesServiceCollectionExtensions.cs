using Microsoft.Extensions.DependencyInjection;
using Task3.Services;

namespace Task3.Extensions;

public static class ServicesServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderService, OrderService>();
        serviceCollection.AddScoped<IProductService, ProductService>();
    }
}