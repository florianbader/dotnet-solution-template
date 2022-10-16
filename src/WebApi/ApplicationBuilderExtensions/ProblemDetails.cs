using Hellang.Middleware.ProblemDetails;

namespace WebApi;

public static partial class ProblemDetails
{
    public static void UseProblemDetails(this IApplicationBuilder app)
        => ProblemDetailsExtensions.UseProblemDetails(app);
}
