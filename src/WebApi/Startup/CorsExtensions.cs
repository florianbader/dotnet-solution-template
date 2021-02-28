using Microsoft.AspNetCore.Builder;

namespace WebApi
{
    public static class CorsExtensions
    {
        public static IApplicationBuilder ConfigureCors(this IApplicationBuilder app) =>
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
    }
}
