using Microsoft.AspNetCore.Builder;
using Serilog.Events;
using WebApi.Diagnostics;

namespace WebApi
{
    public static class SerilogExtensions
    {
        public static IApplicationBuilder UseSerilog(this IApplicationBuilder app)
            => app.UseMiddleware<SerilogExceptionMiddleware>()
                .UseMiddleware<SerilogRequestMiddleware>(LogEventLevel.Information);
    }
}
