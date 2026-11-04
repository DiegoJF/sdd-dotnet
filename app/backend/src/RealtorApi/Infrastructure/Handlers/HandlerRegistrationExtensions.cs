using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RealtorApi.Infrastructure.Handlers;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection RegisterHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(typeof(IHandler).IsAssignableFrom)
            .Distinct();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i != typeof(IHandler))
                .ToArray();

            if (interfaces.Length == 0)
            {
                services.TryAddScoped(handlerType);
                continue;
            }

            foreach (var @interface in interfaces)
            {
                services.TryAddScoped(@interface, handlerType);
            }
        }

        return services;
    }
}
