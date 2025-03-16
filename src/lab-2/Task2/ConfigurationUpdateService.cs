using Task1.Models;
using Task1.Services;

namespace Task2;

public class ConfigurationUpdateService : IDisposable
{
    private readonly IConfigurationService _configurationService;
    private readonly CustomConfigurationProvider _customConfigurationProvider;
    private readonly PeriodicTimer _timer;

    public ConfigurationUpdateService(CustomConfigurationProvider customConfigurationProvider, IConfigurationService configurationService, ConfigurationUpdateOptions options)
    {
        _customConfigurationProvider = customConfigurationProvider;
        _configurationService = configurationService;
        _timer = new PeriodicTimer(options.UpdateInterval);
    }

    public async Task StartAsync(int pageSize, CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken))
        {
            IAsyncEnumerable<ConfigurationItemDto> configurationItems = _configurationService.GetConfigurationsAsync(cancellationToken);
            _customConfigurationProvider.UpdateConfigurations(configurationItems.ToBlockingEnumerable(cancellationToken: cancellationToken));
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}