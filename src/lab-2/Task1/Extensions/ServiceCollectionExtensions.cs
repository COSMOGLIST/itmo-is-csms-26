using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Task1.Models;
using Task1.Services;

namespace Task1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurationService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddScoped<IConfigurationService, ConfigurationService>();
    }

    public static void AddRefitConfigurationService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddRefitClient<IRefitConfigurationService>()
                 .ConfigureHttpClient((provider, client) =>
                 {
                     IOptions<ConfigurationOptions> options = provider.GetRequiredService<IOptions<ConfigurationOptions>>();
                     client.BaseAddress = new Uri(options.Value.BaseUrl);
                 });
        serviceCollection.AddScoped<IConfigurationService, RefitConfigurationService>();
    }
}