using Microsoft.Extensions.Options;
using Task1.Models;
using Task1.Services;
using Task2;

namespace Lab3.BackgroundServices;

public class ConfigurationUpdateService : BackgroundService
{
    private readonly IConfigurationService _configurationService;
    private readonly CustomConfigurationProvider _customConfigurationProvider;
    private readonly TimeSpan _updateInterval;

    public ConfigurationUpdateService(
        CustomConfigurationProvider customConfigurationProvider,
        IConfigurationService configurationService,
        IOptions<ConfigurationUpdateOptions> options)
    {
        _customConfigurationProvider = customConfigurationProvider;
        _configurationService = configurationService;
        _updateInterval = options.Value.UpdateInterval;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        IAsyncEnumerable<ConfigurationItemDto> configurationItems = _configurationService.GetConfigurationsAsync(cancellationToken);
        _customConfigurationProvider.UpdateConfigurations(await configurationItems.ToListAsync(cancellationToken: cancellationToken));
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var periodicTimer = new PeriodicTimer(_updateInterval);
        while (stoppingToken.IsCancellationRequested is false && await periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            IAsyncEnumerable<ConfigurationItemDto> configurationItems = _configurationService.GetConfigurationsAsync(stoppingToken);
            _customConfigurationProvider.UpdateConfigurations(await configurationItems.ToListAsync(cancellationToken: stoppingToken));
        }

        periodicTimer.Dispose();
    }
}