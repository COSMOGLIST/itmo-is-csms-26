using Microsoft.Extensions.Configuration;
using Task1.Models;

namespace Task2;

public class CustomConfigurationProvider : ConfigurationProvider
{
    public void UpdateConfigurations(IEnumerable<ConfigurationItemDto> configurations)
    {
        var newDictionary = configurations.ToDictionary(item => item.Key, item => item.Value);
        if (Data.Count == newDictionary.Count &&
            Data.All(pair =>
                newDictionary.TryGetValue(pair.Key, out string? value) &&
                EqualityComparer<string>.Default.Equals(pair.Value, value)))
        {
            return;
        }

        Data.Clear();
        foreach (string key in newDictionary.Keys)
        {
            Data[key] = newDictionary[key];
        }

        OnReload();
    }
}