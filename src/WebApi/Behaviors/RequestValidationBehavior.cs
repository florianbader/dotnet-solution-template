using Application;
using FluentValidation;
using MediatR;

namespace WebApi.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Distinct()
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .Distinct()
            .ToArray();

        if (failures.Any())
        {
            throw new InvalidInputException(
                failures.Select(f => new ValidationResult(f.ErrorCode, f.ErrorMessage)));
        }

        return next();
    }
}
