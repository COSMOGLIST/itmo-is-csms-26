using BasicImplementation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BasicImplementation.Extensions;

public static class ServicesServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderService, OrderService>();
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<IOrderHistoryService, OrderHistoryService>();
    }
}