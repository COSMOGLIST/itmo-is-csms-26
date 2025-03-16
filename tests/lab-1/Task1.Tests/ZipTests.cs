using FluentAssertions;
using Xunit;

namespace Task1.Tests;

public class ZipTests
{
    [Fact]
    public void Zip_ShouldReturnEnumerableOfSingleElements_WhenCalledWithNoArguments()
    {
        string[] enumerable = { "one", "two", "three" };
        string[] requiredElement1 = { "one" };
        string[] requiredElement2 = { "two" };
        string[] requiredElement3 = { "three" };
        const int requiredSize = 3;

        var result = enumerable.CustomizedZip().ToList();

        result.Should().HaveCount(requiredSize);
        result[0].Should().BeEquivalentTo(requiredElement1);
        result[1].Should().BeEquivalentTo(requiredElement2);
        result[2].Should().BeEquivalentTo(requiredElement3);
    }

    [Fact]
    public async Task ZipAsync_ShouldReturnEnumerableOfSingleElements_WhenCalledWithNoArguments()
    {
        string[] enumerable = { "one", "two", "three" };
        IAsyncEnumerable<string> asyncEnumerable = enumerable.ToAsyncEnumerable();
        string[] requiredElement1 = { "one" };
        string[] requiredElement2 = { "two" };
        string[] requiredElement3 = { "three" };
        const int requiredSize = 3;

        List<IEnumerable<string>> result = await asyncEnumerable.CustomizedZipAsync().ToListAsync();

        result.Should().HaveCount(requiredSize);
        result[0].Should().BeEquivalentTo(requiredElement1);
        result[1].Should().BeEquivalentTo(requiredElement2);
        result[2].Should().BeEquivalentTo(requiredElement3);
    }

    [Theory]
    [MemberData(nameof(DataForZipTest.DataForTest2), MemberType = typeof(DataForZipTest))]
    public void Zip_ShouldReturnProperEnumerableOfEnumerables_WhenCalledWithEnumerablesWithEqualLength(
        IEnumerable<string[]> required,
        IEnumerable<string> enumerable1,
        IEnumerable<string> enumerable2,
        IEnumerable<string> enumerable3)
    {
        IEnumerable<IEnumerable<string>> result = enumerable1.CustomizedZip(enumerable2, enumerable3);
        result.Should().BeEquivalentTo(required);
    }

    [Theory]
    [MemberData(nameof(DataForZipTest.DataForTest2Async), MemberType = typeof(DataForZipTest))]
    public async Task ZipAsync_ShouldReturnProperEnumerableOfEnumerables_WhenCalledWithEnumerablesWithEqualLength(
        IAsyncEnumerable<string[]> required,
        IAsyncEnumerable<string> enumerable1,
        IAsyncEnumerable<string> enumerable2,
        IAsyncEnumerable<string> enumerable3)
    {
        List<IEnumerable<string>> result = await enumerable1.CustomizedZipAsync(enumerable2, enumerable3).ToListAsync();
        List<string[]> requiredAsList = await required.ToListAsync();
        result.Should().BeEquivalentTo(requiredAsList);
    }

    [Theory]
    [MemberData(nameof(DataForZipTest.DataForTest3), MemberType = typeof(DataForZipTest))]
    public void Zip_ShouldReturnProperEnumerableOfEnumerables_WhenCalledWithEnumerablesWithNonEqualLength(
        IEnumerable<string[]> required,
        IEnumerable<string> enumerable1,
        IEnumerable<string> enumerable2,
        IEnumerable<string> enumerable3)
    {
        IEnumerable<IEnumerable<string>> result = enumerable1.CustomizedZip(enumerable2, enumerable3);
        result.Should().BeEquivalentTo(required);
    }

    [Theory]
    [MemberData(nameof(DataForZipTest.DataForTest3Async), MemberType = typeof(DataForZipTest))]
    public async Task ZipAsync_ShouldReturnProperEnumerableOfEnumerables_WhenCalledWithEnumerablesWithNonEqualLength(
        IAsyncEnumerable<string[]> required,
        IAsyncEnumerable<string> enumerable1,
        IAsyncEnumerable<string> enumerable2,
        IAsyncEnumerable<string> enumerable3)
    {
        List<IEnumerable<string>> result = await enumerable1.CustomizedZipAsync(enumerable2, enumerable3).ToListAsync();
        List<string[]> requiredAsList = await required.ToListAsync();
        result.Should().BeEquivalentTo(requiredAsList);
    }
}