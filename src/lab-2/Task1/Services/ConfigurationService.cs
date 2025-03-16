using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Task1.Models;

namespace Task1.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly int _pageSize;
    private readonly string _baseUrl;

    public ConfigurationService(IOptions<ServiceOptions> serviceOptions, IHttpClientFactory httpClientFactory)
    {
        _pageSize = serviceOptions.Value.PageSize;
        _httpClientFactory = httpClientFactory;
        _baseUrl = serviceOptions.Value.BaseUrl;
    }

    public async IAsyncEnumerable<ConfigurationItemDto> GetConfigurationsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();
        string? pageToken = null;
        do
        {
            using HttpResponseMessage response = await httpClient.GetAsync(
                _baseUrl + "/configurations?pageSize=" + _pageSize + "&pageToken=" +
                pageToken,
                cancellationToken);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            ConfigurationResponse? configurationResponse = JsonSerializer.Deserialize<ConfigurationResponse>(json);
            if (configurationResponse != null)
            {
                foreach (ConfigurationItem element in configurationResponse.Items)
                {
                    yield return new ConfigurationItemDto(element.Key, element.Value);
                }
            }

            pageToken = configurationResponse?.PageToken;
            if (pageToken != null) pageToken = Uri.EscapeDataString(pageToken);
        }
        while (pageToken != null);
    }
}