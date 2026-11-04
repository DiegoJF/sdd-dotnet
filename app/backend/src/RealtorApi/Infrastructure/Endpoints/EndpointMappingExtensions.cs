using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Infrastructure.Endpoints;

public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapInfrastructureEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapSliceEndpoints();
        return app;
    }
}
