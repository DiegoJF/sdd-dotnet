using System.Net;
using RealtorApiTests.Infrastructure;

namespace RealtorApiTests.Infrastructure.Endpoints;

public sealed class PropertyAssetsEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PropertyAssetsEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AssetsPropertiesEndpoint_EntregaImagenPublica()
    {
        var response = await _client.GetAsync("/assets/properties/1.png");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Content.Headers.ContentType is not null);
    }
}
