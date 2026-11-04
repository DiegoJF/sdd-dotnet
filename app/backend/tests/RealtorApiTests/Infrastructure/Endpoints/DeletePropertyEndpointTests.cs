using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Endpoints;

public sealed class DeletePropertyEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public DeletePropertyEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_Returns204_AndPropertyIsNoLongerAvailable()
    {
        var targetId = await GetPropertyIdFromEndAsync(0);

        var response = await _client.DeleteAsync($"/api/properties/{targetId:D}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());

        var detail = await _client.GetAsync($"/api/properties/{targetId:D}");
        Assert.Equal(HttpStatusCode.NotFound, detail.StatusCode);

        var listedIds = await GetAllListedIdsAsync();
        Assert.DoesNotContain(targetId, listedIds);
    }

    [Fact]
    public async Task Delete_Returns404ProblemDetails_WhenPropertyWasAlreadyDeleted()
    {
        var targetId = await GetPropertyIdFromEndAsync(1);

        var first = await _client.DeleteAsync($"/api/properties/{targetId:D}");
        var second = await _client.DeleteAsync($"/api/properties/{targetId:D}");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        Assert.Equal("application/problem+json", second.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_Returns404ProblemDetails_WhenPropertyDoesNotExist()
    {
        var countBefore = await CountPropertiesAsync();

        var response = await _client.DeleteAsync("/api/properties/00000000-0000-0000-0000-000000000000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(countBefore, await CountPropertiesAsync());
    }

    [Fact]
    public async Task Delete_Returns400ValidationProblem_WhenIdIsInvalid()
    {
        var response = await _client.DeleteAsync("/api/properties/no-es-uuid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.TryGetProperty("errors", out _));
    }

    [Fact]
    public async Task Delete_RemovesExactlyOneProperty_AndLeavesOthersUnchanged()
    {
        Guid targetId;
        Guid otherId;
        string otherTitle;
        decimal otherPrice;
        int countBefore;

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var properties = await dbContext.Properties.AsNoTracking().OrderByDescending(p => p.Id).Skip(2).Take(2).ToListAsync();
            Assert.Equal(2, properties.Count);

            targetId = properties[0].Id;
            otherId = properties[1].Id;
            otherTitle = properties[1].Title;
            otherPrice = properties[1].Price;
            countBefore = await dbContext.Properties.CountAsync();
        }

        var response = await _client.DeleteAsync($"/api/properties/{targetId:D}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Equal(countBefore - 1, await dbContext.Properties.CountAsync());

            var other = await dbContext.Properties.AsNoTracking().SingleAsync(p => p.Id == otherId);
            Assert.Equal(otherTitle, other.Title);
            Assert.Equal(otherPrice, other.Price);
        }
    }

    [Fact]
    public async Task Delete_KeepsImageFile_WhenPropertyHasImageUrl()
    {
        var targetId = await GetFirstPropertyIdWithImageAsync();

        string imageUrl;
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var property = await dbContext.Properties.AsNoTracking().SingleAsync(p => p.Id == targetId);
            imageUrl = property.ImageUrl!;
        }

        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        var imagePath = Path.Combine(
            environment.WebRootPath,
            "assets",
            "properties",
            Path.GetFileName(imageUrl));
        var existedBefore = File.Exists(imagePath);

        var response = await _client.DeleteAsync($"/api/properties/{targetId:D}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(existedBefore, File.Exists(imagePath));
    }

    // Cada prueba usa una posición distinta porque comparten la misma base de datos del fixture.
    private async Task<Guid> GetPropertyIdFromEndAsync(int offset)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ids = await dbContext.Properties.AsNoTracking().Select(p => p.Id).ToListAsync();
        Assert.True(ids.Count > offset + 4);
        return ids.OrderByDescending(id => id).Skip(offset).First();
    }

    private async Task<Guid> GetFirstPropertyIdWithImageAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ids = await dbContext.Properties.AsNoTracking()
            .Where(p => p.ImageUrl != null)
            .Select(p => p.Id)
            .ToListAsync();
        Assert.NotEmpty(ids);
        return ids.OrderBy(id => id).First();
    }

    private async Task<int> CountPropertiesAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.Properties.CountAsync();
    }

    private async Task<List<Guid>> GetAllListedIdsAsync()
    {
        var response = await _client.GetAsync("/api/properties?Page=1&PageSize=100");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return payload.RootElement
            .GetProperty("items")
            .EnumerateArray()
            .Select(item => item.GetProperty("id").GetGuid())
            .ToList();
    }
}
