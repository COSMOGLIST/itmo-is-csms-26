using Microsoft.Extensions.Configuration;

namespace Task2;

public class CustomConfigurationSource : IConfigurationSource
{
    private readonly CustomConfigurationProvider _customConfigurationProvider;

    public CustomConfigurationSource(CustomConfigurationProvider customConfigurationProvider)
    {
        _customConfigurationProvider = customConfigurationProvider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _customConfigurationProvider;
    }
}