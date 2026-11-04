using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties", HandleAsync)
            .WithName("ListProperties")
            .Produces<PagedPropertyListResponse>()
            .ProducesValidationProblem();
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [AsParameters]
        ListPropertiesQuery query,
        ListPropertiesHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(query, cancellationToken);
        var response = result.ToResponse(httpContext);

        return TypedResults.Ok(response);
    }
}