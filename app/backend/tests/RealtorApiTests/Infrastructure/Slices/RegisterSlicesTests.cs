using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApiTests.Infrastructure.Slices;

public class RegisterSlicesTests
{
    [Fact]
    public void RegisterSlices_DescubreClaseISliceDeTestsYEvitaDuplicados()
    {
        var services = new ServiceCollection();

        services.RegisterSlices(typeof(RegisterSlicesTestSlice).Assembly);
        services.RegisterSlices(typeof(RegisterSlicesTestSlice).Assembly);

        var descriptors = services
            .Where(d => d.ServiceType == typeof(ISlice) && d.ImplementationType == typeof(RegisterSlicesTestSlice))
            .ToList();

        Assert.Single(descriptors);
    }
}
