namespace LINQPadHelpers.Extensions;

public static class DisposableExtensions
{
    /// <summary>Call a func using the disposable argument, then dispose the disposable upon completion. Meant for fluently chaining Using statements to other fluent calls.</summary>
    public static TReturn? Using<TDisposable, TReturn>(
        this TDisposable disposable, 
        Func<TDisposable, TReturn> workFunc
    ) where TDisposable : IDisposable
    {
        using (disposable)
            return workFunc(disposable);
    }
    /// <summary>Call an action using the disposable argument, then dispose the disposable upon completion.</summary>
    public static void Using<TDisposable>(
        this TDisposable disposable, 
        Action<TDisposable> workFunc
    ) where TDisposable : IDisposable
    {
        using (disposable)
            workFunc(disposable);
    }
    /// <summary>Call an async action using the disposable argument, then dispose upon completion.</summary>
    public static async Task Using<TDisposable>(
        this TDisposable disposable, 
        Func<TDisposable,Task> asyncFunc
    ) where TDisposable : IDisposable
    {
        using (disposable)
            await asyncFunc(disposable);
    }
    /// <summary>Call a func using an async disposable, then dispose on completion and return the func result.</summary>
    public static async Task<TReturn>? AwaitUsing<TDisposable, TReturn>(this TDisposable disposable, Func<TDisposable,Task<TReturn>> asyncFunc) where TDisposable : IAsyncDisposable
    {
        await using (disposable)
            return await asyncFunc(disposable);
    }
    /// <summary>Call an async action using an async disposable, then dispose on completion.</summary>
    public static async Task AwaitUsing<TDisposable>(this TDisposable disposable, Func<TDisposable, Task> asyncFunc) where TDisposable : IAsyncDisposable
    {
        await using (disposable)
            await asyncFunc(disposable);
    }
    /// <summary>Call a func using the disposable argument, then dispose the disposable upon completion. Meant for fluently chaining Using statements to other fluent calls.</summary>
    public static async Task<TReturn?> Using<TDisposable, TReturn>(
        this Task<TDisposable> streamAsync,
        Func<TDisposable, TReturn> workFunc
    ) where TDisposable : IDisposable => (await streamAsync).Using(workFunc);
}
