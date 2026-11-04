using RealtorApi.Features.Properties.CreateProperty;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.UpdateStatus;

public sealed class UpdateStatusHandler(
    AppDbContext dbContext,
    ILogger<UpdateStatusHandler> logger) : IHandler
{
    internal async Task<UpdateStatusHandlerResult> HandleAsync(
        UpdateStatusCommand command,
        CancellationToken cancellationToken)
    {
        var property = await dbContext.Properties.FindAsync(
            new object[] { command.Id },
            cancellationToken);

        if (property is null)
        {
            logger.LogInformation(
                "Actualización de estado: propiedad no encontrada con id '{PropertyId}'.",
                command.Id);

            return UpdateStatusHandlerResult.PropertyNotFound();
        }

        property.Status = command.Status;
        property.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Estado de propiedad '{PropertyId}' actualizado correctamente.",
            command.Id);

        return UpdateStatusHandlerResult.Success(property.ToResponse());
    }
}

internal sealed record UpdateStatusHandlerResult(
    bool Succeeded,
    bool NotFound,
    UpdateStatusResponse? Response)
{
    public static UpdateStatusHandlerResult Success(UpdateStatusResponse response)
        => new(true, false, response);

    public static UpdateStatusHandlerResult PropertyNotFound()
        => new(false, true, null);
}