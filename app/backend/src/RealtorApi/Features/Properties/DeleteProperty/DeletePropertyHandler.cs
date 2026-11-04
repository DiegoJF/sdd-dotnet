using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.DeleteProperty;

public sealed class DeletePropertyHandler(
    AppDbContext dbContext,
    ILogger<DeletePropertyHandler> logger) : IHandler
{
    internal async Task<DeletePropertyHandlerResult> HandleAsync(
        DeletePropertyCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id is null)
        {
            throw new InvalidOperationException("DeletePropertyHandler recibió un id no parseado.");
        }

        var propertyId = command.Id.Value;

        var property = await dbContext.Properties.FindAsync(
            new object[] { propertyId },
            cancellationToken);

        if (property is null)
        {
            logger.LogInformation(
                "Eliminación de propiedad: no encontrada. PropertyId={PropertyId}",
                propertyId);

            return DeletePropertyHandlerResult.NotFound(CreateNotFoundProblem());
        }

        // La imagen referenciada por ImageUrl no se modifica (RN-02).
        dbContext.Properties.Remove(property);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogInformation(
                "Eliminación de propiedad: eliminada por otra solicitud concurrente. PropertyId={PropertyId}",
                propertyId);

            return DeletePropertyHandlerResult.NotFound(CreateNotFoundProblem());
        }

        logger.LogInformation(
            "Propiedad eliminada. PropertyId={PropertyId}",
            propertyId);

        return DeletePropertyHandlerResult.Deleted();
    }

    private static ProblemDetails CreateNotFoundProblem() => new()
    {
        Title = "Propiedad no encontrada.",
        Detail = "No existe una propiedad con el id proporcionado.",
        Status = StatusCodes.Status404NotFound
    };
}

internal sealed record DeletePropertyHandlerResult(ProblemDetails? Problem)
{
    public static DeletePropertyHandlerResult Deleted() => new((ProblemDetails?)null);

    public static DeletePropertyHandlerResult NotFound(ProblemDetails problem) => new(problem);
}
