using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class PropertyConfigurationTests
{
    [Fact]
    public void PropertyConfiguration_DefineConversionYRestriccionesEsperadas()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new AppDbContext(options);

        var entityType = context.Model.FindEntityType(typeof(Property));
        Assert.NotNull(entityType);

        var statusProperty = entityType!.FindProperty(nameof(Property.Status));
        Assert.NotNull(statusProperty);
        Assert.Equal(typeof(string), statusProperty!.GetProviderClrType());
        Assert.Equal(32, statusProperty.GetMaxLength());

        var priceProperty = entityType.FindProperty(nameof(Property.Price));
        Assert.NotNull(priceProperty);
        Assert.Equal("12,2", priceProperty!.GetPrecision() + "," + priceProperty.GetScale());

        var imageUrlProperty = entityType.FindProperty(nameof(Property.ImageUrl));
        Assert.NotNull(imageUrlProperty);
        Assert.Equal(260, imageUrlProperty!.GetMaxLength());
    }
}
