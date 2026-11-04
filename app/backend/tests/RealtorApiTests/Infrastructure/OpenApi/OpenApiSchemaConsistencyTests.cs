using System.Text.Json;

namespace RealtorApiTests.Infrastructure.OpenApi;

public sealed class OpenApiSchemaConsistencyTests
{
    [Fact]
    public void GeneratedDocument_ContainsRequiredSharedSchemas()
    {
        using var document = OpenApiDocumentReader.ReadGeneratedV1Document();

        var schemas = document.RootElement
            .GetProperty("components")
            .GetProperty("schemas");

        Assert.True(schemas.TryGetProperty("PropertyStatus", out _));
        Assert.True(schemas.TryGetProperty("ProblemDetails", out _));
        Assert.True(schemas.TryGetProperty("HttpValidationProblemDetails", out _));
    }

    [Fact]
    public void GeneratedDocument_UsesRefsForProblemSchemasWithoutInlineDuplication()
    {
        using var document = OpenApiDocumentReader.ReadGeneratedV1Document();

        var paths = document.RootElement.GetProperty("paths");

        var referencedSchemas = new HashSet<string>(StringComparer.Ordinal);
        foreach (var path in paths.EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                if (!operation.Value.TryGetProperty("responses", out var responses) || responses.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                foreach (var response in responses.EnumerateObject())
                {
                    if (!response.Value.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    foreach (var mediaType in content.EnumerateObject())
                    {
                        if (!mediaType.Value.TryGetProperty("schema", out var schema))
                        {
                            continue;
                        }

                        if (schema.TryGetProperty("$ref", out var refValue))
                        {
                            referencedSchemas.Add(refValue.GetString() ?? string.Empty);
                        }
                    }
                }
            }
        }

        Assert.Contains("#/components/schemas/ProblemDetails", referencedSchemas);
        Assert.Contains("#/components/schemas/HttpValidationProblemDetails", referencedSchemas);
    }
}
