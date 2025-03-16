using Microsoft.Extensions.Primitives;
using Task1.Models;
using Xunit;

namespace Task2.Tests;

public static class ProviderTests
{
    [Fact]
    public static void Provider_ShouldUpdateConfigurationAndAddReceivedElement_WhenEmptyAndGetsElement()
    {
        string key = "key";
        string value = "value";
        IEnumerable<ConfigurationItemDto> configurationItems = new[] { new ConfigurationItemDto(key, value) };
        var customConfigurationProvider = new CustomConfigurationProvider();

        IChangeToken reloadTokenBefore = customConfigurationProvider.GetReloadToken();
        customConfigurationProvider.UpdateConfigurations(configurationItems);
        IChangeToken reloadTokenAfter = customConfigurationProvider.GetReloadToken();

        Assert.NotEqual(reloadTokenBefore, reloadTokenAfter);
        Assert.True(customConfigurationProvider.TryGet(key, out string? _));
    }

    [Fact]
    public static void Provider_ShouldNotUpdateConfigurationAndAddReceivedElement_WhenHasThisElement()
    {
        string key = "key";
        string value = "value";
        IEnumerable<ConfigurationItemDto> configurationItems = new[] { new ConfigurationItemDto(key, value) };
        var customConfigurationProvider = new CustomConfigurationProvider();
        customConfigurationProvider.UpdateConfigurations(configurationItems);

        IChangeToken reloadTokenBefore = customConfigurationProvider.GetReloadToken();
        customConfigurationProvider.UpdateConfigurations(configurationItems);
        IChangeToken reloadTokenAfter = customConfigurationProvider.GetReloadToken();

        Assert.Equal(reloadTokenBefore, reloadTokenAfter);
    }

    [Fact]
    public static void Provider_ShouldUpdateConfigurationAndChangeValue_WhenHasThisKeyWithOtherValue()
    {
        string key = "key";
        string value = "value";
        IEnumerable<ConfigurationItemDto> configurationItems = new[] { new ConfigurationItemDto(key, value) };
        var customConfigurationProvider = new CustomConfigurationProvider();
        customConfigurationProvider.UpdateConfigurations(configurationItems);
        string value1 = "value1";
        IEnumerable<ConfigurationItemDto> configurationItems1 = new[] { new ConfigurationItemDto(key, value1) };

        IChangeToken reloadTokenBefore = customConfigurationProvider.GetReloadToken();
        customConfigurationProvider.UpdateConfigurations(configurationItems1);
        IChangeToken reloadTokenAfter = customConfigurationProvider.GetReloadToken();

        Assert.NotEqual(reloadTokenBefore, reloadTokenAfter);
        Assert.True(customConfigurationProvider.TryGet(key, out string? result));
        Assert.Equal(result, value1);
    }

    [Fact]
    public static void Provider_ShouldUpdateConfiguration_WhenHasElementAndGetsEmptyConfiguration()
    {
        string key = "key";
        string value = "value";
        IEnumerable<ConfigurationItemDto> configurationItems = new[] { new ConfigurationItemDto(key, value) };
        var customConfigurationProvider = new CustomConfigurationProvider();
        customConfigurationProvider.UpdateConfigurations(configurationItems);
        IEnumerable<ConfigurationItemDto> configurationItems1 = Array.Empty<ConfigurationItemDto>();

        IChangeToken reloadTokenBefore = customConfigurationProvider.GetReloadToken();
        customConfigurationProvider.UpdateConfigurations(configurationItems1);
        IChangeToken reloadTokenAfter = customConfigurationProvider.GetReloadToken();

        Assert.NotEqual(reloadTokenBefore, reloadTokenAfter);
    }
}