using RealtorApi.Features.Properties.CreateProperty;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyHandler(
    AppDbContext dbContext,
    IWebHostEnvironment environment,
    ILogger<UpdatePropertyHandler> logger) : IHandler
{
    internal async Task<UpdatePropertyHandlerResult> HandleAsync(
        UpdatePropertyCommand command,
        CancellationToken cancellationToken)
    {
        var property = await dbContext.Properties.FindAsync(
            new object[] { command.Id },
            cancellationToken);

        if (property is null)
        {
            logger.LogInformation(
                "Actualización de propiedad: no encontrada con id '{PropertyId}'.",
                command.Id);

            return UpdatePropertyHandlerResult.PropertyNotFound();
        }

        StoredPropertyImage? storedImage = null;

        try
        {
            if (command.Image is not null)
            {
                storedImage = await CreatePropertyImageStorage.SaveAsync(
                    command.Image,
                    environment,
                    cancellationToken);
            }

            property.Title = command.Title;
            property.Description = command.Description;
            property.Address = command.Address;
            property.Price = command.Price;
            property.Status = command.Status;
            property.UpdatedAt = DateTimeOffset.UtcNow;

            if (storedImage is not null)
            {
                property.ImageUrl = storedImage.RelativePath;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Propiedad '{PropertyId}' actualizada correctamente.",
                command.Id);

            return UpdatePropertyHandlerResult.Success(property.ToResponse());
        }
        catch (Exception exception) when (command.Image is not null)
        {
            if (storedImage is not null)
            {
                try
                {
                    CreatePropertyImageStorage.TryDelete(storedImage.PhysicalPath);
                }
                catch (Exception deleteException)
                {
                    logger.LogWarning(
                        deleteException,
                        "No fue posible limpiar el archivo de imagen '{ImagePath}' tras fallo en actualización de propiedad.",
                        storedImage.PhysicalPath);
                }
            }

            logger.LogError(
                exception,
                "Fallo interno al almacenar imagen durante la actualización de propiedad '{PropertyId}'.",
                command.Id);

            return UpdatePropertyHandlerResult.StorageFailure();
        }
    }
}

internal sealed record UpdatePropertyHandlerResult(
    bool Succeeded,
    bool NotFound,
    bool StorageFailed,
    UpdatePropertyResponse? Response)
{
    public static UpdatePropertyHandlerResult Success(UpdatePropertyResponse response)
        => new(true, false, false, response);

    public static UpdatePropertyHandlerResult PropertyNotFound()
        => new(false, true, false, null);

    public static UpdatePropertyHandlerResult StorageFailure()
        => new(false, false, true, null);
}
