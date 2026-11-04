using System.Net;
using System.Net.Http.Json;
using RealtorApiTests.Infrastructure;

namespace RealtorApiTests.Infrastructure.Endpoints;

public class HealthSliceTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthSliceTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthMigradoASlice_RespondeOkConPayloadEsperado()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<HealthPayload>();
        Assert.NotNull(payload);
        Assert.Equal("ok", payload!.Status);
    }

    private sealed class HealthPayload
    {
        public string Status { get; set; } = string.Empty;
    }
}
