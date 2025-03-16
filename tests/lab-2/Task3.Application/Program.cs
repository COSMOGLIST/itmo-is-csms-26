#pragma warning disable CA1506
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task1.Extensions;
using Task1.Models;
using Task1.Services;
using Task2;
using Task3.Extensions;
using Task3.Models;
using Task3.Services;

var provider = new CustomConfigurationProvider();
var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json").Add(new CustomConfigurationSource(provider));
IConfigurationRoot configuration = builder.Build();
var serviceCollection = new ServiceCollection();
serviceCollection.Configure<ServiceOptions>(o => o.PageSize = 10);
serviceCollection.Configure<ConfigurationOptions>(configuration.GetSection("OuterService"));
serviceCollection.AddRefitConfigurationService();
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
IConfigurationService? configurationService = serviceProvider.GetService<IConfigurationService>();
if (configurationService != null)
{
    IAsyncEnumerable<ConfigurationItemDto> items = configurationService.GetConfigurationsAsync(CancellationToken.None);
    provider.UpdateConfigurations(items.ToBlockingEnumerable());
}

var postgresConnectionOptions = new PostgresConnectionOptions();
configuration.GetSection("Postgres").Bind(postgresConnectionOptions);

serviceCollection.Configure<PostgresConnectionOptions>(options =>
{
    options.Host = postgresConnectionOptions.Host;
    options.Port = postgresConnectionOptions.Port;
    options.Username = postgresConnectionOptions.Username;
    options.Password = postgresConnectionOptions.Password;
});
serviceCollection.AddMigration();
serviceCollection.AddRepositories();
serviceCollection.AddServices();

serviceProvider = serviceCollection.BuildServiceProvider();

serviceProvider.RunMigration();

IProductService? productService = serviceProvider.GetService<IProductService>();
IOrderService? orderService = serviceProvider.GetService<IOrderService>();

if (productService != null && orderService != null)
{
    long milkId = await productService.CreateAsync(new Product("MOLOKO", 1000), CancellationToken.None);
    long abobaId = await productService.CreateAsync(new Product("ABOBA", 500), CancellationToken.None);
    long bananaId = await productService.CreateAsync(new Product("BANANA", 1), CancellationToken.None);

    long id = await orderService.CreateAsync(new Order(DateTime.Now, "Author"), CancellationToken.None);
    AddOrderItemResultType bananaResultType = await orderService.AddOrderItemAsync(new OrderItem(id, bananaId, 3), CancellationToken.None);
    AddOrderItemResultType abobaResultType = await orderService.AddOrderItemAsync(new OrderItem(id, abobaId, 2), CancellationToken.None);
    AddOrderItemResultType milkResultType = await orderService.AddOrderItemAsync(new OrderItem(id, milkId, 1), CancellationToken.None);

    if (milkResultType is AddOrderItemResultType.Success result)
    {
        await orderService.RemoveOrderItemAsync(id, result.OrderItemId, CancellationToken.None);
    }

    await orderService.TransferToWorkAsync(id, CancellationToken.None);
    await orderService.CompleteOrderAsync(id, CancellationToken.None);
    IAsyncEnumerable<OrderHistory> orderHistory = orderService.GetHistoryByFilterAsync(0, 10, new OrderHistoryFilterQuery(id), CancellationToken.None);
}