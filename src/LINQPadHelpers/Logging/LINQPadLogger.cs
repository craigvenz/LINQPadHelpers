using LINQPad;
using Microsoft.Extensions.Logging;

namespace LINQPadHelpers.Logging;

public class LINQPadLogger : ILogger
{
    internal IExternalScopeProvider? ScopeProvider { get; set; }
    internal DumpContainer Container { get; set; }
    private readonly string _name;
    internal LINQPadLogger(string name, DumpContainer container, IExternalScopeProvider? scopeProvider)
    {
        ScopeProvider = scopeProvider;
        Container = container;
        _name = name;
    }
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => ScopeProvider?.Push(state) ?? NullScope.Instance;
    public void Log<TState>(LogLevel logLevel, 
                            EventId eventId, 
                            TState state, 
                            Exception? exception, 
                            Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        ArgumentNullException.ThrowIfNull(formatter);
        var logEntry = new MyLogEntry<TState>(logLevel, _name, eventId, state, exception, formatter, false);
        Container.AppendContent(Util.WithHeading(logEntry, logEntry.Category));
    }
}
