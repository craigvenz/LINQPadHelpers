using LINQPad;

namespace LINQPadHelpers.Extensions;

public static class TaskExtensions
{
    /// <summary>Dump an async task - doesn't require async on the method.</summary>
    public static T? DumpAsync<T>(this Task<T> obj, string name = "") 
        => Util.Try(() => obj.Await().Dump(name), ex => default!, true);
    /// <summary>Await this async task that returns a value.</summary>
    public static T Await<T>(this Task<T> task) => task.GetAwaiter().GetResult();
    /// <summary>Await this async task that returns nothing.</summary>
    public static void Await(this Task task) => task.GetAwaiter().GetResult();
}