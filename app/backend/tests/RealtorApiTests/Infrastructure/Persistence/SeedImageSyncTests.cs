using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class SeedImageSyncTests
{
    [Fact]
    public async Task DatabaseSeeder_SincronizaImagenesHaciaDestinoFinal()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest();
        workspace.WriteStatuses(new { value = "Available", description = "Disponible" });
        workspace.WriteProperties(
            new
            {
                id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                title = "Casa imagen",
                description = "Desc",
                address = "Addr",
                price = 1300m,
                status = "Available",
                bedroomCount = 1,
                bathroomCount = 1,
                areaSquareMeters = 60m,
                imageUrl = "img-sync.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("img-sync.png");

        var dbPath = Path.Combine(workspace.RootPath, "sync-images.db");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, new SeedManifestLoader(workspace.RootPath), NullLogger.Instance);

        var copiedImagePath = Path.Combine(workspace.ImagesTargetPath, "img-sync.png");
        Assert.True(File.Exists(copiedImagePath));
    }
}
