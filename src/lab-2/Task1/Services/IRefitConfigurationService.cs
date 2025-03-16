using Refit;
using Task1.Models;

namespace Task1.Services;

public interface IRefitConfigurationService
{
    [Get("/configurations?pageSize={pageSize}&pageToken={pageToken}")]
    Task<ConfigurationResponse?> GetConfigurationsAsync(string? pageToken, int pageSize, CancellationToken cancellationToken);
}