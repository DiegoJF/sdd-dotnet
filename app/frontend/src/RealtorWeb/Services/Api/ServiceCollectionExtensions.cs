using Microsoft.Extensions.Options;
using Refit;
using RealtorWeb.Services.Api.Properties;
using RealtorWeb.Services.Api.System;

namespace RealtorWeb.Services.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRealtorApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RealtorApiOptions>(configuration.GetSection(RealtorApiOptions.SectionName));

        services.AddRefitClient<IBackendHealthApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                client.BaseAddress = ResolveBaseAddress(sp);
            });

        services.AddRefitClient<IPropertiesApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                client.BaseAddress = ResolveBaseAddress(sp);
            });

        return services;
    }

    private static Uri ResolveBaseAddress(IServiceProvider serviceProvider)
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<RealtorApiOptions>>()
            .Value;

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseAddress))
        {
            throw new InvalidOperationException(
                $"La configuracion '{RealtorApiOptions.SectionName}:BaseUrl' debe ser una URI absoluta valida.");
        }

        return baseAddress;
    }
}
