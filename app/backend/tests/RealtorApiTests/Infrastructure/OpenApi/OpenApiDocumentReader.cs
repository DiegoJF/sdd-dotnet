using System.Text.Json;

namespace RealtorApiTests.Infrastructure.OpenApi;

public static class OpenApiDocumentReader
{
    public static JsonDocument ReadGeneratedV1Document()
    {
        var filePath = Path.Combine(GetRepositoryRoot(), "app", "backend", "src", "RealtorApi", "wwwroot", "openapi", "v1.json");
        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException($"No se encontró el documento OpenAPI generado: {filePath}");
        }

        return JsonDocument.Parse(File.ReadAllText(filePath));
    }

    public static HashSet<string> ReadDocumentedOperationSet(JsonDocument document)
    {
        var operations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!document.RootElement.TryGetProperty("paths", out var pathsElement) || pathsElement.ValueKind != JsonValueKind.Object)
        {
            return operations;
        }

        foreach (var path in pathsElement.EnumerateObject())
        {
            var normalizedPath = OpenApiRouteNormalizer.Normalize(path.Name);
            foreach (var operation in path.Value.EnumerateObject())
            {
                var method = operation.Name.ToUpperInvariant();
                if (method is "GET" or "POST" or "PUT" or "PATCH" or "DELETE" or "OPTIONS" or "HEAD")
                {
                    operations.Add($"{method} {normalizedPath}");
                }
            }
        }

        return operations;
    }

    private static string GetRepositoryRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../../"));
    }
}
