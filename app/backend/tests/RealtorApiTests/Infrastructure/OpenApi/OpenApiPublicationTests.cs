using System.Net;
using System.Text.Json;
using RealtorApiTests.Infrastructure;

namespace RealtorApiTests.Infrastructure.OpenApi;

public sealed class OpenApiPublicationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OpenApiPublicationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OpenApiV1Endpoint_Returns200WithJsonContentType()
    {
        var response = await _client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApiV1Document_IncludesAllPublicEndpointsInScope()
    {
        var response = await _client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var operations = OpenApiDocumentReader.ReadDocumentedOperationSet(json);

        Assert.Contains("GET /health", operations);
        Assert.Contains("GET /api/properties", operations);
        Assert.Contains("POST /api/properties", operations);
        Assert.Contains("GET /api/properties/{id}", operations);
        Assert.Contains("PUT /api/properties/{id}", operations);
    }
}
