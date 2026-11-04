using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class PropertyImageUrlConsistencyTests
{
    [Fact]
    public async Task DatabaseSeeder_PersisteImageUrlPublicaConsistente()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest(publicImageBasePath: "/assets/properties");
        workspace.WriteStatuses(new { value = "Available", description = "Disponible" });
        workspace.WriteProperties(
            new
            {
                id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                title = "Casa image url",
                description = "Desc",
                address = "Addr",
                price = 1400m,
                status = "Available",
                bedroomCount = 2,
                bathroomCount = 1,
                areaSquareMeters = 75m,
                imageUrl = "url-consistency.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("url-consistency.png");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={Path.Combine(workspace.RootPath, "url-consistency.db")}")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, new SeedManifestLoader(workspace.RootPath), NullLogger.Instance);

        var property = await context.Properties.SingleAsync();

        Assert.StartsWith("/assets/properties/", property.ImageUrl, StringComparison.Ordinal);
        Assert.DoesNotContain("support/", property.ImageUrl, StringComparison.OrdinalIgnoreCase);
    }
}
