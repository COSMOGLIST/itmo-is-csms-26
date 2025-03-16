namespace Task1.Tests;

public static class DataForZipTest
{
    public static IEnumerable<object[]> DataForTest2()
    {
        yield return new object[] { new[] { new[] { "one", "one1", "one2" }, new[] { "two", "two1", "two2" }, new[] { "three", "three1", "three2" } }, new[] { "one", "two", "three" }, new[] { "one1", "two1", "three1" }, new[] { "one2", "two2", "three2" } };
    }

    public static IEnumerable<object[]> DataForTest2Async()
    {
        string[][] required =
        {
            new[] { "one", "one1", "one2" },
            new[] { "two", "two1", "two2" },
            new[] { "three", "three1", "three2" },
        };
        IAsyncEnumerable<string[]> asyncRequired = required.ToAsyncEnumerable();
        string[] enumerable1 = { "one", "two", "three" };
        IAsyncEnumerable<string> enumerable1Async = enumerable1.ToAsyncEnumerable();
        string[] enumerable2 = { "one1", "two1", "three1" };
        IAsyncEnumerable<string> enumerable2Async = enumerable2.ToAsyncEnumerable();
        string[] enumerable3 = { "one2", "two2", "three2" };
        IAsyncEnumerable<string> enumerable3Async = enumerable3.ToAsyncEnumerable();
        yield return new object[] { asyncRequired, enumerable1Async, enumerable2Async, enumerable3Async };
    }

    public static IEnumerable<object[]> DataForTest3()
    {
        yield return new object[] { new[] { new[] { "one", "one1", "one2" }, new[] { "two", "two1", "two2" } }, new[] { "one", "two", "three" }, new[] { "one1", "two1", "three1" }, new[] { "one2", "two2" } };
    }

    public static IEnumerable<object[]> DataForTest3Async()
    {
        string[][] required =
        {
            new[] { "one", "one1", "one2" },
            new[] { "two", "two1", "two2" },
        };
        IAsyncEnumerable<string[]> asyncRequired = required.ToAsyncEnumerable();
        string[] enumerable1 = { "one", "two", "three" };
        IAsyncEnumerable<string> enumerable1Async = enumerable1.ToAsyncEnumerable();
        string[] enumerable2 = { "one1", "two1", "three1" };
        IAsyncEnumerable<string> enumerable2Async = enumerable2.ToAsyncEnumerable();
        string[] enumerable3 = { "one2", "two2" };
        IAsyncEnumerable<string> enumerable3Async = enumerable3.ToAsyncEnumerable();
        yield return new object[] { asyncRequired, enumerable1Async, enumerable2Async, enumerable3Async };
    }
}