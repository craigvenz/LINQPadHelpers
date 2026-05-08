using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LINQPadHelpers.Misc;

//https://dev.to/nikolicbojan/log-httpclient-request-and-response-based-on-custom-conditions-in-net-core-412f
public class TraceLogHandler(
    IHttpContextAccessor httpContextAccessor,
    Func<HttpContext?, HttpResponseMessage, bool> shouldLog,
    TraceLogParts parts = TraceLogParts.All)
    : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var logPayloads = false;

        HttpResponseMessage? response = null;
        try
        {
            response = await base.SendAsync(request, cancellationToken);

            logPayloads = shouldLog(_httpContextAccessor.HttpContext, response);
        }
        catch (Exception)
        {
            logPayloads = true;
            throw;
        }
        finally
        {
            if (logPayloads && parts != TraceLogParts.None && _httpContextAccessor.HttpContext != null)
            {
                var logger = _httpContextAccessor.HttpContext.RequestServices.GetService<ILogger<TraceLogHandler>>();

                if ((parts & TraceLogParts.RequestHeaders) > 0)
                    logger?.LogTrace("Request Headers:\n{request}", request);
                if (request?.Content != null && (parts & TraceLogParts.RequestBody) > 0)
                    logger?.LogTrace("Request Body:\n---\n{content}\n---", await request.Content.ReadAsStringAsync(cancellationToken));
                if ((parts & TraceLogParts.ResponseHeaders) > 0)
                    logger?.LogTrace("Response Headers:\n{response}", response);
                if (response?.Content != null && (parts & TraceLogParts.ResponseBody) > 0)
                    logger?.LogTrace("Response Body:\n---\n{content}\n---", await response.Content.ReadAsStringAsync(cancellationToken));
            }
        }

        return response;
    }
}
[Flags]
public enum TraceLogParts
{
    /// <summary>Log nothing.</summary>
    None = 0,
    /// <summary>Log request headers.</summary>
    RequestHeaders = 1,
    /// <summary>Log request body.</summary>
    RequestBody = 2,
    /// <summary>Log response headers.</summary>
    ResponseHeaders = 4,
    /// <summary>Log response body.</summary>
    ResponseBody = 8,
    /// <summary>Shortcut for Request Headers and Body.</summary>
    RequestAll = RequestHeaders | RequestBody,
    /// <summary>Shortcut for Response Headers and Body.</summary>
    ResponseAll = ResponseHeaders | ResponseBody,
    /// <summary>Shortcut for Request and Response Headers and Body.</summary>
    All = RequestAll | ResponseAll
}