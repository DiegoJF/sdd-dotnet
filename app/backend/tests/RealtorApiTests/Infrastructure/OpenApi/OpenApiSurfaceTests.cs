using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RealtorApiTests.Infrastructure.OpenApi;

public sealed class OpenApiSurfaceTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public OpenApiSurfaceTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Runtime_DoesNotExposeSwaggerUiOrRedocInteractiveRoutes()
    {
        var routes = _factory.Services.GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(routes, route => route.Contains("swagger", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(routes, route => route.Contains("redoc", StringComparison.OrdinalIgnoreCase));
    }
}
