using LINQPad;
using Microsoft.Extensions.Logging;

namespace LINQPadHelpers.Logging;

public sealed class LINQPadLoggerProvider(DumpContainer container) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
        => new LINQPadLogger(categoryName, container, NullScope.Instance);
    public void Dispose() { }
}
