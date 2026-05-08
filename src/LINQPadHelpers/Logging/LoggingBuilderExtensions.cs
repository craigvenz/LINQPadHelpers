using LINQPad;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace LINQPadHelpers.Logging;

public static class LoggingBuilderExtensions
{
    /// <summary>Adds a logger to the Logging builder which outputs logs to the supplied LINQPad DumpContainer.</summary>
    public static ILoggingBuilder AddLINQPadLogger(this ILoggingBuilder builder, DumpContainer output)
    {
        builder.Services.TryAddSingleton(output);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LINQPadLoggerProvider>());
        return builder;
    }
}
