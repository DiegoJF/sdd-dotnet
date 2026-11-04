using System.Reflection;
using FluentValidation;

namespace RealtorApi.Infrastructure.Validation;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationConventions(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);
        return services;
    }
}
