using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Persistence.Migrations;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class PropertiesMigrationSchemaTests
{
    [Fact]
    public void Migration_UniqueSpec003_ContieneUpDownYSnapshotEsperado()
    {
        var migrationType = typeof(AddPropertiesPersistenceSeeding);
        var upMethod = migrationType.GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
        var downMethod = migrationType.GetMethod("Down", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(upMethod);
        Assert.NotNull(downMethod);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=fake;Username=fake;Password=fake")
            .Options;

        using var context = new AppDbContext(options);

        var propertyType = context.Model.FindEntityType(typeof(Property));
        var statusType = context.Model.FindEntityType(typeof(PropertyStatus));

        Assert.NotNull(propertyType);
        Assert.NotNull(statusType);
        Assert.Equal("properties", propertyType!.GetTableName());
        Assert.Equal("property_statuses", statusType!.GetTableName());
    }
}
