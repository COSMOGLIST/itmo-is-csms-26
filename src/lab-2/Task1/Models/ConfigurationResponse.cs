using System.Text.Json.Serialization;

namespace Task1.Models;

public class ConfigurationResponse
{
    public ConfigurationResponse(IEnumerable<ConfigurationItem> items, string? pageToken)
    {
        Items = items;
        PageToken = pageToken;
    }

    [JsonPropertyName("items")]
    public IEnumerable<ConfigurationItem> Items { get; set; }

    [JsonPropertyName("pageToken")]
    public string? PageToken { get; set; }
}