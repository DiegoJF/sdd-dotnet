using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class MissingImageSelectiveSkipTests
{
    [Fact]
    public async Task DatabaseSeeder_OmiteSoloPropiedadConImagenFaltante()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest();
        workspace.WriteStatuses(new { value = "Available", description = "Disponible" });
        workspace.WriteProperties(
            new
            {
                id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                title = "Casa válida",
                description = "Desc",
                address = "Addr",
                price = 1500m,
                status = "Available",
                bedroomCount = 2,
                bathroomCount = 1,
                areaSquareMeters = 85m,
                imageUrl = "exists.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            },
            new
            {
                id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                title = "Casa inválida",
                description = "Desc",
                address = "Addr",
                price = 1600m,
                status = "Available",
                bedroomCount = 2,
                bathroomCount = 1,
                areaSquareMeters = 95m,
                imageUrl = "missing.png",
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = (DateTimeOffset?)null
            });
        workspace.WriteImage("exists.png");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={Path.Combine(workspace.RootPath, "missing-image.db")}")
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, new SeedManifestLoader(workspace.RootPath), NullLogger.Instance);

        Assert.Equal(1, await context.Properties.CountAsync());
        Assert.NotNull(await context.Properties.SingleOrDefaultAsync(property => property.Id == Guid.Parse("55555555-5555-5555-5555-555555555555")));
        Assert.Null(await context.Properties.SingleOrDefaultAsync(property => property.Id == Guid.Parse("66666666-6666-6666-6666-666666666666")));
    }
}
