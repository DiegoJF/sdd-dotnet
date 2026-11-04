using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class DatabaseSeederIdempotencyTests
{
    [Fact]
    public async Task DatabaseSeeder_EsIdempotenteYNoDuplicaRegistros()
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
                id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                title = "Casa 1",
                description = "Desc 1",
                address = "Addr 1",
                price = 1000m,
                status = "Available",
                bedroomCount = 2,
                bathroomCount = 1,
                areaSquareMeters = 90m,
                imageUrl = "1.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("1.png");

        var dbPath = Path.Combine(workspace.RootPath, "seed-idempotency.db");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var loader = new SeedManifestLoader(workspace.RootPath);

        await DatabaseSeeder.SeedAsync(context, loader, NullLogger.Instance);
        await DatabaseSeeder.SeedAsync(context, loader, NullLogger.Instance);

        var statusCount = await context.PropertyStatuses.CountAsync();
        var propertiesCount = await context.Properties.CountAsync();

        Assert.Equal(3, statusCount);
        Assert.Equal(1, propertiesCount);

        var stored = await context.Properties.SingleAsync(property => property.Id == Guid.Parse("11111111-1111-1111-1111-111111111111"));
        Assert.Equal(PropertyStatusValue.Available, stored.Status);
        Assert.Equal("/assets/properties/1.png", stored.ImageUrl);
    }
}
