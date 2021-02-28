using System;
using Application.Exceptions;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;

namespace WebApi
{
    public static class ProblemDetailsConfiguration
    {
        public static void Configure(ProblemDetailsOptions options)
        {
            options.Map<InvalidInputException>(ex => new ErrorsProblemDetails(ex));
            options.Map<Exception>(ex => new ApplicationProblemDetails(ex, StatusCodes.Status500InternalServerError));
        }
    }
}
