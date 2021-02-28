using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace WebApi.Diagnostics
{
    public class SerilogRequestMiddleware
    {
        private const string MessageTemplate = "Request {RequestMethod} {RequestPath} elapsed in {Elapsed:0.0000} ms with status {StatusCode}";

        private readonly LogEventLevel _logEventLevel;
        private readonly RequestDelegate _next;

        public SerilogRequestMiddleware(RequestDelegate next, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logEventLevel = logEventLevel;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return InvokeInternalAsync(httpContext);
        }

        private async Task InvokeInternalAsync(HttpContext httpContext)
        {
            var start = Stopwatch.GetTimestamp();

            try
            {
                await _next(httpContext);
            }
            finally
            {
                var elapsedMs = (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency;
                var statusCode = httpContext.Response?.StatusCode;

                Log.Logger
                    .ForRequest(httpContext.Request)
                    .Write(
                        _logEventLevel,
                        MessageTemplate,
                        httpContext.Request.Method,
                        httpContext.Request.Path.ToString(),
                        statusCode,
                        elapsedMs);
            }
        }
    }
}
