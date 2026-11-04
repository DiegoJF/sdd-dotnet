using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApiTests.Infrastructure.OpenApi;

public static class OpenApiEndpointInventory
{
    public static HashSet<string> ReadRuntimeOperationSet(IServiceProvider services)
    {
        var endpointDataSources = services.GetServices<EndpointDataSource>().ToArray();
        var operations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var endpoint in endpointDataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>())
        {
            var methods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;
            if (methods is null || methods.Count == 0)
            {
                continue;
            }

            var route = NormalizeRoute(endpoint.RoutePattern.RawText);
            if (!IsPublicFunctionalRoute(route))
            {
                continue;
            }

            foreach (var method in methods)
            {
                operations.Add($"{method.ToUpperInvariant()} {route}");
            }
        }

        return operations;
    }

    public static bool IsPublicFunctionalRoute(string route)
    {
        return route.Equals("/health", StringComparison.OrdinalIgnoreCase)
            || route.StartsWith("/api/properties", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRoute(string? rawRoute)
    {
        if (string.IsNullOrWhiteSpace(rawRoute))
        {
            return string.Empty;
        }

        var route = rawRoute.StartsWith('/') ? rawRoute : "/" + rawRoute;
        return OpenApiRouteNormalizer.Normalize(route);
    }
}
