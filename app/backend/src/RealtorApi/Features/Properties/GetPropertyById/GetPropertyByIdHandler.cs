using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler(
    AppDbContext dbContext,
    ILogger<GetPropertyByIdHandler> logger) : IHandler
{
    internal async Task<GetPropertyByIdHandlerResult> HandleAsync(
        GetPropertyByIdQuery query,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (query.Id is null)
        {
            throw new InvalidOperationException("GetPropertyByIdHandler recibió un id no parseado.");
        }

        var property = await dbContext.Properties.FindAsync(
            new object[] { query.Id.Value },
            cancellationToken);

        if (property is null)
        {
            logger.LogInformation(
                "Consulta de propiedad por id: no encontrada. PropertyId={PropertyId}",
                query.Id.Value);

            return GetPropertyByIdHandlerResult.NotFound(
                new ProblemDetails
                {
                    Title = "Propiedad no encontrada.",
                    Detail = "No existe una propiedad con el id proporcionado.",
                    Status = StatusCodes.Status404NotFound
                });
        }

        var response = property.ToResponse(httpContext);
        return GetPropertyByIdHandlerResult.Success(response);
    }
}

internal sealed record GetPropertyByIdHandlerResult(
    PropertyDetailResponse? Response,
    ProblemDetails? Problem)
{
    public static GetPropertyByIdHandlerResult Success(PropertyDetailResponse response)
        => new(response, null);

    public static GetPropertyByIdHandlerResult NotFound(ProblemDetails problem)
        => new(null, problem);
}
