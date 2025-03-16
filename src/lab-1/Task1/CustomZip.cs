namespace Task1;

public static class CustomZip
{
    public static IEnumerable<IEnumerable<T>> CustomizedZip<T>(this IEnumerable<T> first, params IEnumerable<T>[] others)
    {
        IEnumerable<IEnumerator<T>> allEnumerators = GetEnumerators(first, others).ToArray();
        try
        {
            while (allEnumerators.All(enumerator => enumerator.MoveNext()))
            {
                yield return allEnumerators.Select(enumerator => enumerator.Current).ToArray();
            }
        }
        finally
        {
            foreach (IEnumerator<T> enumerator in allEnumerators)
            {
                enumerator.Dispose();
            }
        }
    }

    private static IEnumerable<IEnumerator<T>> GetEnumerators<T>(IEnumerable<T> first, params IEnumerable<T>[] others)
    {
        yield return first.GetEnumerator();
        foreach (IEnumerable<T> enumerable in others)
        {
            yield return enumerable.GetEnumerator();
        }
    }
}