using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RealtorApi.Infrastructure.Slices;

public static class SliceRegistrationExtensions
{
    public static IServiceCollection RegisterSlices(this IServiceCollection services, Assembly assembly)
    {
        var sliceTypes = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(typeof(ISlice).IsAssignableFrom)
            .Distinct();

        foreach (var sliceType in sliceTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ISlice), sliceType));
        }

        return services;
    }
}
