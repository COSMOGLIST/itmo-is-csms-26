using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Task3.Models;
using Task3.Repositories;

namespace Task3.Extensions;

public static class RepositoryServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(serviceProvider =>
        {
            IOptions<PostgresConnectionOptions> configuration = serviceProvider.GetRequiredService<IOptions<PostgresConnectionOptions>>();
            var builder = new NpgsqlDataSourceBuilder(configuration.Value.ToConnectionString());
            builder.MapEnum<OrderState>(pgName: "order_state");
            builder.MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");

            NpgsqlDataSource dataSource = builder.Build();
            return dataSource;
        });

        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
        serviceCollection.AddScoped<IOrderItemRepository, OrderItemRepository>();
        serviceCollection.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
    }
}