using System.Text.RegularExpressions;

namespace RealtorApiTests.Infrastructure.OpenApi;

public static partial class OpenApiRouteNormalizer
{
    [GeneratedRegex("\\{([^}:]+):[^}]+\\}", RegexOptions.Compiled)]
    private static partial Regex RouteConstraintRegex();

    public static string Normalize(string route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return route;
        }

        var normalized = RouteConstraintRegex().Replace(route, "{$1}");
        return normalized.Trim();
    }
}
