using Serilog;

namespace WebApi.Diagnostics;

public class SerilogExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public SerilogExceptionMiddleware(RequestDelegate next) => _next = next ?? throw new ArgumentNullException(nameof(next));

    public Task Invoke(HttpContext httpContext) => InvokeInternalAsync(httpContext ?? throw new ArgumentNullException(nameof(httpContext)));

    private async Task InvokeInternalAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            Log.Logger
                .ForRequest(httpContext.Request)
                .Error(ex, "Request exception occurred");
            throw;
        }
    }
}
