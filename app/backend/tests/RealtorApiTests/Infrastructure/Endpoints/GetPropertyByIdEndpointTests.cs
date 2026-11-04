using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Endpoints;

public sealed class GetPropertyByIdEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private static readonly Guid ExistingPropertyWithImageId = Guid.Parse("a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1002");
    private static readonly Guid ExistingPropertyWithoutImageId = Guid.Parse("a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1001");

    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GetPropertyByIdEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_Returns200_WithPublicContractWithoutPaginationMetadata()
    {
        var response = await _client.GetAsync($"/api/properties/{ExistingPropertyWithImageId:D}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = payload.RootElement;

        Assert.Equal(ExistingPropertyWithImageId, root.GetProperty("id").GetGuid());
        Assert.True(root.TryGetProperty("title", out _));
        Assert.True(root.TryGetProperty("description", out _));
        Assert.True(root.TryGetProperty("address", out _));
        Assert.True(root.TryGetProperty("price", out _));
        Assert.True(root.TryGetProperty("status", out _));
        Assert.True(root.TryGetProperty("bedroomCount", out _));
        Assert.True(root.TryGetProperty("bathroomCount", out _));
        Assert.True(root.TryGetProperty("areaSquareMeters", out _));
        Assert.True(root.TryGetProperty("imageUrl", out var imageUrl));

        var imageUrlValue = imageUrl.GetString();
        Assert.NotNull(imageUrlValue);
        Assert.StartsWith("http://localhost/assets/properties/", imageUrlValue, StringComparison.Ordinal);

        Assert.False(root.TryGetProperty("items", out _));
        Assert.False(root.TryGetProperty("page", out _));
        Assert.False(root.TryGetProperty("pageSize", out _));
        Assert.False(root.TryGetProperty("totalItems", out _));
    }

    [Fact]
    public async Task GetById_Returns404ProblemDetails_WhenPropertyDoesNotExist()
    {
        var response = await _client.GetAsync("/api/properties/00000000-0000-0000-0000-000000000000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetById_Returns400ProblemDetails_WhenIdIsInvalid()
    {
        var response = await _client.GetAsync("/api/properties/not-a-uuid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetById_Returns200WithNullImageUrl_WhenPropertyHasNoImage()
    {
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var property = await dbContext.Properties.FindAsync(ExistingPropertyWithoutImageId);

            Assert.NotNull(property);

            property.ImageUrl = null;
            await dbContext.SaveChangesAsync();
        }

        var response = await _client.GetAsync($"/api/properties/{ExistingPropertyWithoutImageId:D}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var imageUrl = payload.RootElement.GetProperty("imageUrl");

        Assert.Equal(JsonValueKind.Null, imageUrl.ValueKind);
    }

    [Fact]
    public async Task GetById_BuildsAbsoluteImageUrl_WithRequestHostAndScheme()
    {
        var customClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://qa.realtor.local")
        });

        var response = await customClient.GetAsync($"/api/properties/{ExistingPropertyWithImageId:D}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var imageUrl = payload.RootElement.GetProperty("imageUrl").GetString();

        Assert.NotNull(imageUrl);
        Assert.StartsWith("https://qa.realtor.local/assets/properties/", imageUrl, StringComparison.Ordinal);
    }
}
