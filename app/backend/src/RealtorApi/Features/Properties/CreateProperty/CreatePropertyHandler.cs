using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyHandler(
    AppDbContext dbContext,
    IWebHostEnvironment environment,
    ILogger<CreatePropertyHandler> logger) : IHandler
{
    internal async Task<CreatePropertyHandlerResult> HandleAsync(
        CreatePropertyCommand command,
        CancellationToken cancellationToken)
    {
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

            var property = command.ToProperty(storedImage?.RelativePath, DateTimeOffset.UtcNow);

            dbContext.Properties.Add(property);
            await dbContext.SaveChangesAsync(cancellationToken);

            return CreatePropertyHandlerResult.Success(property.ToResponse());
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
                        "No fue posible limpiar el archivo de imagen '{ImagePath}' tras fallo en alta de propiedad.",
                        storedImage.PhysicalPath);
                }
            }

            logger.LogError(
                exception,
                "Fallo interno al almacenar imagen durante el alta de propiedad.");

            return CreatePropertyHandlerResult.StorageFailure();
        }
    }
}

internal sealed record CreatePropertyHandlerResult(
    bool Succeeded,
    bool StorageFailed,
    CreatePropertyResponse? Response)
{
    public static CreatePropertyHandlerResult Success(CreatePropertyResponse response)
    {
        return new CreatePropertyHandlerResult(true, false, response);
    }

    public static CreatePropertyHandlerResult StorageFailure()
    {
        return new CreatePropertyHandlerResult(false, true, null);
    }
}
