using System;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Behaviors;

namespace WebApi
{
    public static class MediatorExtensions
    {
        public static void AddMediator(this IServiceCollection services)
        {
            services.AddTransientFromAssembly(typeof(ApplicationException), typeof(IValidator<>));

            services.AddAutoMapper(typeof(ApplicationException));

            services.AddMediatR(typeof(ApplicationException));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        }
    }
}
