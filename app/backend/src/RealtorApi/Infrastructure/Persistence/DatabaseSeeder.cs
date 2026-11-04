using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context, SeedManifestLoader manifestLoader, ILogger logger)
    {
        SeedInternalAsync(context, manifestLoader, logger, CancellationToken.None).GetAwaiter().GetResult();
    }

    public static Task SeedAsync(
        AppDbContext context,
        SeedManifestLoader manifestLoader,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        return SeedInternalAsync(context, manifestLoader, logger, cancellationToken);
    }

    private static async Task SeedInternalAsync(
        AppDbContext context,
        SeedManifestLoader manifestLoader,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var manifest = manifestLoader.LoadManifest();
        ValidateManifest(manifest);

        var statuses = manifestLoader.LoadStatuses(manifest);
        var properties = manifestLoader.LoadProperties(manifest);

        await UpsertStatusesAsync(context, statuses, cancellationToken);

        var sourceImagesDirectory = manifestLoader.ResolvePath(manifest.ImagesSourceDirectory);
        var targetImagesDirectory = manifestLoader.ResolvePath(manifest.ImagesTargetDirectory);

        if (!Directory.Exists(sourceImagesDirectory))
        {
            throw new InvalidOperationException($"No existe el directorio de imágenes de seed: '{sourceImagesDirectory}'.");
        }

        Directory.CreateDirectory(targetImagesDirectory);

        SyncImages(sourceImagesDirectory, targetImagesDirectory);

        var allowedStatusValues = statuses
            .Select(status => status.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var normalizedPublicImageBasePath = NormalizePublicImageBasePath(manifest.PublicImageBasePath);

        foreach (var seedProperty in properties)
        {
            if (!allowedStatusValues.Contains(seedProperty.Status))
            {
                logger.LogError(
                    "Se omite propiedad {PropertyId} por estado no permitido en seed: {Status}.",
                    seedProperty.Id,
                    seedProperty.Status);
                continue;
            }

            if (!Enum.TryParse<PropertyStatusValue>(seedProperty.Status, true, out var parsedStatus))
            {
                logger.LogError(
                    "Se omite propiedad {PropertyId} por estado no mapeable al enum: {Status}.",
                    seedProperty.Id,
                    seedProperty.Status);
                continue;
            }

            var imageFileName = Path.GetFileName(seedProperty.ImageUrl);
            var sourceImagePath = Path.Combine(sourceImagesDirectory, imageFileName);

            if (!File.Exists(sourceImagePath))
            {
                logger.LogError(
                    "Se omite propiedad {PropertyId} porque falta la imagen '{ImageFileName}'.",
                    seedProperty.Id,
                    imageFileName);
                continue;
            }

            var existingProperty = await context.Properties
                .SingleOrDefaultAsync(property => property.Id == seedProperty.Id, cancellationToken);

            var publicImageUrl = $"{normalizedPublicImageBasePath}/{imageFileName}";

            if (existingProperty is null)
            {
                context.Properties.Add(new Property
                {
                    Id = seedProperty.Id,
                    Title = seedProperty.Title,
                    Description = seedProperty.Description,
                    Address = seedProperty.Address,
                    Price = seedProperty.Price,
                    Status = parsedStatus,
                    BedroomCount = seedProperty.BedroomCount,
                    BathroomCount = seedProperty.BathroomCount,
                    AreaSquareMeters = seedProperty.AreaSquareMeters,
                    ImageUrl = publicImageUrl,
                    CreatedAt = seedProperty.CreatedAt,
                    UpdatedAt = seedProperty.UpdatedAt
                });

                continue;
            }

            existingProperty.Title = seedProperty.Title;
            existingProperty.Description = seedProperty.Description;
            existingProperty.Address = seedProperty.Address;
            existingProperty.Price = seedProperty.Price;
            existingProperty.Status = parsedStatus;
            existingProperty.BedroomCount = seedProperty.BedroomCount;
            existingProperty.BathroomCount = seedProperty.BathroomCount;
            existingProperty.AreaSquareMeters = seedProperty.AreaSquareMeters;
            existingProperty.ImageUrl = publicImageUrl;
            existingProperty.CreatedAt = seedProperty.CreatedAt;
            existingProperty.UpdatedAt = seedProperty.UpdatedAt;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task UpsertStatusesAsync(
        AppDbContext context,
        IReadOnlyList<PropertyStatusSeedModel> statuses,
        CancellationToken cancellationToken)
    {
        foreach (var seedStatus in statuses)
        {
            var existingStatus = await context.PropertyStatuses
                .SingleOrDefaultAsync(status => status.Value == seedStatus.Value, cancellationToken);

            if (existingStatus is null)
            {
                context.PropertyStatuses.Add(new PropertyStatus
                {
                    Value = seedStatus.Value,
                    Description = seedStatus.Description
                });

                continue;
            }

            existingStatus.Description = seedStatus.Description;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static void SyncImages(string sourceDirectory, string targetDirectory)
    {
        foreach (var imagePath in Directory.GetFiles(sourceDirectory))
        {
            var imageFileName = Path.GetFileName(imagePath);
            var targetPath = Path.Combine(targetDirectory, imageFileName);
            File.Copy(imagePath, targetPath, overwrite: true);
        }
    }

    private static string NormalizePublicImageBasePath(string basePath)
    {
        var normalized = string.IsNullOrWhiteSpace(basePath)
            ? "/assets/properties"
            : basePath.Trim();

        if (!normalized.StartsWith('/'))
        {
            normalized = '/' + normalized;
        }

        return normalized.TrimEnd('/');
    }

    private static void ValidateManifest(SeedManifestModel manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.StatusesFile) ||
            string.IsNullOrWhiteSpace(manifest.PropertiesFile) ||
            string.IsNullOrWhiteSpace(manifest.ImagesSourceDirectory) ||
            string.IsNullOrWhiteSpace(manifest.ImagesTargetDirectory) ||
            string.IsNullOrWhiteSpace(manifest.PublicImageBasePath))
        {
            throw new InvalidOperationException("seed-manifest.json no contiene todas las claves requeridas.");
        }
    }
}
