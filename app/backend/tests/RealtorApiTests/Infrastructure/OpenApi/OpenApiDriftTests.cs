using RealtorApiTests.Infrastructure;

namespace RealtorApiTests.Infrastructure.OpenApi;

public sealed class OpenApiDriftTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public OpenApiDriftTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void RuntimeEndpoints_AreAlignedWithDocumentedOperations_UsingRouteNormalization()
    {
        using var document = OpenApiDocumentReader.ReadGeneratedV1Document();

        var runtime = OpenApiEndpointInventory.ReadRuntimeOperationSet(_factory.Services);
        var documented = OpenApiDocumentReader.ReadDocumentedOperationSet(document)
            .Where(operation => operation.StartsWith("GET /health", StringComparison.OrdinalIgnoreCase)
                || operation.Contains("/api/properties", StringComparison.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingInDocument = runtime.Except(documented, StringComparer.OrdinalIgnoreCase).ToArray();
        var missingInRuntime = documented.Except(runtime, StringComparer.OrdinalIgnoreCase).ToArray();

        Assert.True(missingInDocument.Length == 0,
            "Faltan operaciones en documento: " + string.Join(", ", missingInDocument));

        Assert.True(missingInRuntime.Length == 0,
            "Hay operaciones documentadas sin endpoint runtime: " + string.Join(", ", missingInRuntime));
    }
}
