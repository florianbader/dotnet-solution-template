using Microsoft.AspNetCore.Builder;

namespace WebApi
{
    public static class Cors
    {
        public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
            => app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
    }
}
