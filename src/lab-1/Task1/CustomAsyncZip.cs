namespace Task1;

public static class CustomAsyncZip
{
    public static async IAsyncEnumerable<IEnumerable<T>> CustomizedZipAsync<T>(this IAsyncEnumerable<T> first, params IAsyncEnumerable<T>[] others)
    {
        IEnumerable<IAsyncEnumerator<T>> allEnumerators = GetEnumerators(first, others).ToArray();
        try
        {
            while (await IsContinue(allEnumerators))
            {
                yield return allEnumerators.Select(enumerator => enumerator.Current).ToArray();
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<T> enumerator in allEnumerators)
            {
                await enumerator.DisposeAsync();
            }
        }
    }

    private static async Task<bool> IsContinue<T>(IEnumerable<IAsyncEnumerator<T>> enumerators)
    {
        foreach (IAsyncEnumerator<T> enumerator in enumerators)
        {
            if (await enumerator.MoveNextAsync().AsTask() is false)
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<IAsyncEnumerator<T>> GetEnumerators<T>(IAsyncEnumerable<T> first, params IAsyncEnumerable<T>[] others)
    {
        yield return first.GetAsyncEnumerator();
        foreach (IAsyncEnumerable<T> enumerable in others)
        {
            yield return enumerable.GetAsyncEnumerator();
        }
    }
}