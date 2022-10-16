using Application;
using Hellang.Middleware.ProblemDetails;

namespace WebApi;

public static partial class ProblemDetails
{
    public static void AddProblemDetails(this IServiceCollection services)
        => services.AddProblemDetails(options =>
        {
            options.Map<GenericApplicationException>(ex => new ApplicationProblemDetails(ex, StatusCodes.Status500InternalServerError));
            options.Map<EntityNotFoundException>(ex => new ApplicationProblemDetails(ex, StatusCodes.Status404NotFound));
            options.Map<InvalidInputException>(ex => new ErrorsProblemDetails(ex));
            options.Map<Exception>(ex => new ApplicationProblemDetails(ex, StatusCodes.Status500InternalServerError));
        });
}
