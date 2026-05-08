using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LINQPadHelpers.Misc;

public static class HttpClientBuilderExtensions
{
    /// <summary>Adds an instance of the TraceLogHandler to a HTTP client configuration.</summary>
    public static IHttpClientBuilder AddTraceLogHandler(this IHttpClientBuilder builder, Func<HttpContext?,HttpResponseMessage,bool>? shouldLog = null, TraceLogParts parts = TraceLogParts.All) 
        => builder.AddHttpMessageHandler(services => new TraceLogHandler(services.GetRequiredService<IHttpContextAccessor>(), shouldLog ?? ((_,_) => true), parts));
}
