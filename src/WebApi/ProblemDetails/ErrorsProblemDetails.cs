using System.Net;
using Application;
using Hellang.Middleware.ProblemDetails;

namespace WebApi;

public class ErrorsProblemDetails : StatusCodeProblemDetails
{
    public ErrorsProblemDetails(InvalidInputException exception)
        : base((int)HttpStatusCode.BadRequest)
    {
        Detail = exception.Message;
        Errors = exception.Errors;
    }

    public IEnumerable<ValidationResult> Errors { get; set; }
}
