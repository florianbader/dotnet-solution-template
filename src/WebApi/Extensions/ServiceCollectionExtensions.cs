using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTransientFromAssembly(this IServiceCollection services, Type assemblyType, Type interfaceType)
        {
            var types = interfaceType.IsGenericType
                ? assemblyType.Assembly.GetTypes()
                    .Where(t => GetFirstGenericInterfaceType(t, interfaceType) is not null)
                : assemblyType.Assembly.GetTypes()
                    .Where(t => interfaceType.IsAssignableFrom(t));

            foreach (var type in types)
            {
                var matchingInterface = interfaceType.IsGenericType
                    ? GetFirstGenericInterfaceType(type, interfaceType)
                    : interfaceType;

                if (matchingInterface is not null)
                {
                    services.AddTransient(matchingInterface, type);
                }
            }
        }

        private static Type? GetFirstGenericInterfaceType(Type type, Type interfaceType)
            => type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
    }
}
