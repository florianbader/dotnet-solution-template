using Microsoft.AspNetCore.Builder;

namespace WebApi
{
    public static partial class OpenApi
    {
        public static void UseOpenApi(this IApplicationBuilder app)
            => app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1"));
    }
}
