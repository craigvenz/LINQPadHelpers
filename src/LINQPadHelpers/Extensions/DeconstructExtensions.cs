#nullable disable

namespace LINQPadHelpers.Extensions;

public static class DeconstructExtensions
{
    /// <summary>Deconstruct an enumerable into a two-value tuple.</summary>
    public static void Deconstruct<T>(this IEnumerable<T> val, out T first, out T second)
    {
        ArgumentNullException.ThrowIfNull(val);
        var list = val.Take(2).ToList();
        var e = 0;
        (first, second) = (list.ElementAtOrDefault(e++), list.ElementAtOrDefault(e));
    }
    /// <summary>Deconstruct an enumerable into a three-value tuple.</summary>
    public static void Deconstruct<T>(this IEnumerable<T> val, out T first, out T second, out T third)
    {
        ArgumentNullException.ThrowIfNull(val);
        var list = val.Take(3).ToList();
        var e = 0;
        (first, second, third) = (list.ElementAtOrDefault(e++),
                                  list.ElementAtOrDefault(e++),
                                  list.ElementAtOrDefault(e));
    }
    /// <summary>Deconstruct an enumerable into a four-value tuple.</summary>
    public static void Deconstruct<T>(this IEnumerable<T> val, out T first, out T second, out T third, out T fourth)
    {
        ArgumentNullException.ThrowIfNull(val);
        var list = val.Take(4).ToList();
        var e = 0;
        (first, second, third, fourth) = (list.ElementAtOrDefault(e++),
                                          list.ElementAtOrDefault(e++),
                                          list.ElementAtOrDefault(e++),
                                          list.ElementAtOrDefault(e));
    }
    /// <summary>Deconstruct an enumerable into a five-value tuple.</summary>
    public static void Deconstruct<T>(this IEnumerable<T> val, out T first, out T second, out T third, out T fourth, out T fifth)
    {
        ArgumentNullException.ThrowIfNull(val);
        var list = val.Take(5).ToList();
        var e = 0;
        (first, second, third, fourth, fifth) = (list.ElementAtOrDefault(e++),
                                                 list.ElementAtOrDefault(e++),
                                                 list.ElementAtOrDefault(e++),
                                                 list.ElementAtOrDefault(e++),
                                                 list.ElementAtOrDefault(e));
    }
}
#nullable restore