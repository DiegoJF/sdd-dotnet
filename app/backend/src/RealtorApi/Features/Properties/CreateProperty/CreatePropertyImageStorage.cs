using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace RealtorApi.Features.Properties.CreateProperty;

internal sealed record StoredPropertyImage(
    string RelativePath,
    string PhysicalPath);

internal static class CreatePropertyImageStorage
{
    private const string PropertyAssetsFolder = "assets/properties";

    public static async Task<StoredPropertyImage> SaveAsync(
        IFormFile image,
        IWebHostEnvironment environment,
        CancellationToken cancellationToken)
    {
        var webRootPath = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        var targetDirectory = Path.Combine(webRootPath, "assets", "properties");
        Directory.CreateDirectory(targetDirectory);

        var storedFileName = CreateStoredFileName(image.FileName);
        var physicalPath = Path.Combine(targetDirectory, storedFileName);

        await using var fileStream = new FileStream(
            physicalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await image.CopyToAsync(fileStream, cancellationToken);

        return new StoredPropertyImage(
            BuildRelativePublicPath(storedFileName),
            physicalPath);
    }

    public static string CreateStoredFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        return $"{Guid.NewGuid():N}{extension}";
    }

    public static string BuildRelativePublicPath(string storedFileName)
    {
        return $"/{PropertyAssetsFolder}/{storedFileName}";
    }

    public static void TryDelete(string physicalPath)
    {
        if (!File.Exists(physicalPath))
        {
            return;
        }

        File.Delete(physicalPath);
    }
}
