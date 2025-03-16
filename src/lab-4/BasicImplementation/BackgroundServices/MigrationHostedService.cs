using BasicImplementation.Extensions;
using Microsoft.Extensions.Hosting;

namespace BasicImplementation.BackgroundServices;

public class MigrationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceProvider.RunMigration();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}