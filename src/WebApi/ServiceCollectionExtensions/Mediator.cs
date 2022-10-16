using Application;
using FluentValidation;
using MediatR;
using WebApi.Behaviors;

namespace WebApi;

public static class Mediator
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddTransientFromAssembly(typeof(GenericApplicationException), typeof(IValidator<>));

        services.AddMediatR(typeof(GenericApplicationException));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
    }
}
