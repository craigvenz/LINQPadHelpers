using System.Data.Linq;

namespace LINQPadHelpers.Extensions;

public static class OtherExtensions
{
    private static readonly Bogus.Faker Faker = new();

    public static T PickRandom<T>(this IEnumerable<T> items) => Faker.PickRandom(items);

    public static EntitySet<T> ToEntitySet<T>(this IEnumerable<T> source) where T : class
    {
        var es = new EntitySet<T>();
        es.AddRange(source);
        return es;
    }

    /// <summary>List.ForEach for any enumerable.</summary>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
            action.Invoke(item);
        return collection;
    }

    public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> collection, Func<TIn, TOut> selector)
        => collection.Select(selector);

    /// <summary>Return a collection of exceptions from an exception chain. Handles inners, or aggregate exceptions.</summary>
    public static IEnumerable<Exception> ToEnumerable(this Exception root)
    {
        var q = new Queue<Exception>();
        q.Enqueue(root);
        while (q.Count > 0)
        {
            var ex = q.Dequeue();
            yield return ex;
            if (ex is AggregateException aex)
            {
                foreach (var x in aex.InnerExceptions)
                    q.Enqueue(x);
            }
            else
            {
                if (ex.InnerException != null)
                    q.Enqueue(ex.InnerException);
            }
        }
    }
}