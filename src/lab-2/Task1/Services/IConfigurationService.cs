using Task1.Models;

namespace Task1.Services;

public interface IConfigurationService
{
    IAsyncEnumerable<ConfigurationItemDto> GetConfigurationsAsync(CancellationToken cancellationToken);
}