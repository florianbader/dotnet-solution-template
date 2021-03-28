using Application;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public static class Mapper
    {
        public static void AddMapper(this IServiceCollection services) => services.AddAutoMapper(typeof(GenericApplicationException));
    }
}
