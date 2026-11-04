using System.Text.Json;

namespace RealtorApiTests.Infrastructure.Persistence;

internal sealed class SeedTestWorkspace : IDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public string RootPath { get; }

    public string SupportSeedDataPath => Path.Combine(RootPath, "support", "seed-data");

    public string ImagesSourcePath => Path.Combine(SupportSeedDataPath, "images", "properties");

    public string ImagesTargetPath => Path.Combine(RootPath, "wwwroot", "assets", "properties");

    public SeedTestWorkspace()
    {
        RootPath = Path.Combine(Path.GetTempPath(), "realtor-seed-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(SupportSeedDataPath);
        Directory.CreateDirectory(ImagesSourcePath);
        Directory.CreateDirectory(ImagesTargetPath);
    }

    public void WriteManifest(
        string statusesFile = "support/seed-data/properties-statuses.json",
        string propertiesFile = "support/seed-data/properties.json",
        string imagesSourceDirectory = "support/seed-data/images/properties",
        string imagesTargetDirectory = "wwwroot/assets/properties",
        string publicImageBasePath = "/assets/properties")
    {
        var manifest = new
        {
            version = "1.0.0",
            statusesFile,
            propertiesFile,
            imagesSourceDirectory,
            imagesTargetDirectory,
            publicImageBasePath
        };

        var manifestPath = Path.Combine(SupportSeedDataPath, "seed-manifest.json");
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, _jsonOptions));
    }

    public void WriteStatuses(params object[] statuses)
    {
        var path = Path.Combine(SupportSeedDataPath, "properties-statuses.json");
        File.WriteAllText(path, JsonSerializer.Serialize(statuses, _jsonOptions));
    }

    public void WriteProperties(params object[] properties)
    {
        var path = Path.Combine(SupportSeedDataPath, "properties.json");
        File.WriteAllText(path, JsonSerializer.Serialize(properties, _jsonOptions));
    }

    public void WriteImage(string fileName)
    {
        var path = Path.Combine(ImagesSourcePath, fileName);
        File.WriteAllBytes(path, [1, 2, 3, 4]);
    }

    public void Dispose()
    {
        if (!Directory.Exists(RootPath))
        {
            return;
        }

        try
        {
            Directory.Delete(RootPath, recursive: true);
        }
        catch
        {
            // Ignorar problemas de cleanup en pruebas.
        }
    }
}
