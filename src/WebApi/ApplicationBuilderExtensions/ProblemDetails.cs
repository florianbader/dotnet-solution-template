using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;

namespace WebApi
{
    public static partial class ProblemDetails
    {
        public static void UseProblemDetails(this IApplicationBuilder app)
            => ProblemDetailsExtensions.UseProblemDetails(app);
    }
}
