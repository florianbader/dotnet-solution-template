using ILogger = Serilog.ILogger;

namespace WebApi.Diagnostics;

public static class SerilogRequestExtensions
{
    private static readonly string[] Headers = { "Content-Type", "Content-Length", "User-Agent" };

    public static ILogger ForRequest(this ILogger logger, HttpRequest request)
    {
        var loggedHeaders = request.Headers
            .Where(h => Headers.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        return logger
            .ForContext("RequestHeaders", loggedHeaders, destructureObjects: true)
            .ForContext("RequestHost", request.Host)
            .ForContext("RequestProtocol", request.Protocol);
    }
}
