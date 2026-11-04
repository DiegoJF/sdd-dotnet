using RealtorApi.Infrastructure.Slices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RealtorApiTests.Infrastructure.Slices;

public sealed class RegisterSlicesTestSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/test-slice", () => TypedResults.Ok());
    }
}
