using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Infrastructure.Endpoints;

public sealed class HealthSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => TypedResults.Ok(new HealthResponse("ok")))
            .WithName("GetHealth")
            .Produces<HealthResponse>();
    }
}

public sealed record HealthResponse(string Status);
