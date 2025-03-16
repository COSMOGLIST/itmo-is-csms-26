using System.Text.Json.Serialization;

namespace Task1.Models;

public class ConfigurationItem
{
    public ConfigurationItem(string key, string value)
    {
        Key = key;
        Value = value;
    }

    [JsonPropertyName("key")]
    public string Key { get; }

    [JsonPropertyName("value")]
    public string Value { get; }
}