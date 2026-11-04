using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class StartupAsyncSeedingTests
{
    [Fact]
    public async Task EnsureCreatedAsync_DisparaUseAsyncSeedingConfigurado()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest();
        workspace.WriteStatuses(
            new { value = "Available", description = "Disponible" },
            new { value = "Rented", description = "Rentada" },
            new { value = "Maintenance", description = "Mantenimiento" });
        workspace.WriteProperties(
            new
            {
                id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                title = "Casa startup",
                description = "Desc startup",
                address = "Addr startup",
                price = 1200m,
                status = "Available",
                bedroomCount = 2,
                bathroomCount = 2,
                areaSquareMeters = 95m,
                imageUrl = "startup.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("startup.png");

        var loader = new SeedManifestLoader(workspace.RootPath);
        var dbPath = Path.Combine(workspace.RootPath, "startup-seeding.db");
        var seeded = false;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                seeded = true;
                await DatabaseSeeder.SeedAsync((AppDbContext)context, loader, NullLogger.Instance, cancellationToken);
            })
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        Assert.True(seeded);
        Assert.True(await context.Properties.AnyAsync());
    }
}
