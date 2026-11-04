using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class InvalidStatusSelectiveSkipTests
{
    [Fact]
    public async Task DatabaseSeeder_OmitePropiedadConEstadoFueraDeCatalogo()
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
                id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                title = "Casa válida",
                description = "Desc",
                address = "Addr",
                price = 1000m,
                status = "Available",
                bedroomCount = 1,
                bathroomCount = 1,
                areaSquareMeters = 70m,
                imageUrl = "valid-status.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            },
            new
            {
                id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                title = "Casa inválida",
                description = "Desc",
                address = "Addr",
                price = 1100m,
                status = "Archived",
                bedroomCount = 1,
                bathroomCount = 1,
                areaSquareMeters = 70m,
                imageUrl = "invalid-status.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("valid-status.png");
        workspace.WriteImage("invalid-status.png");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={Path.Combine(workspace.RootPath, "invalid-status.db")}")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, new SeedManifestLoader(workspace.RootPath), NullLogger.Instance);

        Assert.Equal(1, await context.Properties.CountAsync());
        Assert.NotNull(await context.Properties.SingleOrDefaultAsync(property => property.Id == Guid.Parse("77777777-7777-7777-7777-777777777777")));
        Assert.Null(await context.Properties.SingleOrDefaultAsync(property => property.Id == Guid.Parse("88888888-8888-8888-8888-888888888888")));
    }
}
