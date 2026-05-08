using Microsoft.Extensions.Logging;

namespace LINQPadHelpers.Logging;

internal sealed class NullScope : IDisposable, IExternalScopeProvider
{
    public static NullScope Instance { get; } = new NullScope();
    private NullScope() { }
    public void Dispose() { }
    public void ForEachScope<TState>(Action<object?, TState> callback, TState state) { }
    public IDisposable Push(object? state) => this;
}
