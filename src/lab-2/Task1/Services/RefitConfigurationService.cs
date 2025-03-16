using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Task1.Models;

namespace Task1.Services;

public class RefitConfigurationService : IConfigurationService
{
    private readonly IRefitConfigurationService _refitConfigurationService;
    private readonly int _pageSize;

    public RefitConfigurationService(IOptions<ServiceOptions> serviceOptions, IRefitConfigurationService refitConfigurationService)
    {
        _pageSize = serviceOptions.Value.PageSize;
        _refitConfigurationService = refitConfigurationService;
    }

    public async IAsyncEnumerable<ConfigurationItemDto> GetConfigurationsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string? pageToken = null;
        do
        {
            ConfigurationResponse? configurationResponse =
                await _refitConfigurationService.GetConfigurationsAsync(pageToken, _pageSize, cancellationToken);
            if (configurationResponse != null)
            {
                foreach (ConfigurationItem element in configurationResponse.Items)
                {
                    yield return new ConfigurationItemDto(element.Key, element.Value);
                }
            }

            pageToken = configurationResponse?.PageToken;
        }
        while (pageToken != null);
    }
}