using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace RealtorApi.Infrastructure.Persistence;

public sealed class SeedManifestLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _contentRootPath;
    private readonly bool _useBaseDirectoryFallback;

    public SeedManifestLoader(IHostEnvironment hostEnvironment)
    {
        _contentRootPath = hostEnvironment.ContentRootPath;
        _useBaseDirectoryFallback = true;
    }

    public SeedManifestLoader(string contentRootPath, bool useBaseDirectoryFallback = true)
    {
        _contentRootPath = contentRootPath;
        _useBaseDirectoryFallback = useBaseDirectoryFallback;
    }

    public SeedManifestModel LoadManifest()
    {
        var manifestPath = ResolveExistingPath("support/seed-data/seed-manifest.json");
        return ReadRequiredJsonFile<SeedManifestModel>(manifestPath, "No se pudo cargar seed-manifest.json");
    }

    public IReadOnlyList<PropertyStatusSeedModel> LoadStatuses(SeedManifestModel manifest)
    {
        var statusesPath = ResolveExistingPath(manifest.StatusesFile);
        return ReadRequiredJsonFile<List<PropertyStatusSeedModel>>(statusesPath, "No se pudo cargar properties-statuses.json");
    }

    public IReadOnlyList<PropertySeedModel> LoadProperties(SeedManifestModel manifest)
    {
        var propertiesPath = ResolveExistingPath(manifest.PropertiesFile);
        return ReadRequiredJsonFile<List<PropertySeedModel>>(propertiesPath, "No se pudo cargar properties.json");
    }

    public string ResolvePath(string relativeOrAbsolutePath)
    {
        if (string.IsNullOrWhiteSpace(relativeOrAbsolutePath))
        {
            throw new InvalidOperationException("La ruta no puede ser vacía en seed-manifest.json.");
        }

        if (Path.IsPathRooted(relativeOrAbsolutePath))
        {
            return relativeOrAbsolutePath;
        }

        var normalizedRelativePath = relativeOrAbsolutePath.Replace('/', Path.DirectorySeparatorChar);
        var contentRootCandidate = Path.GetFullPath(Path.Combine(_contentRootPath, normalizedRelativePath));

        return contentRootCandidate;
    }

    private string ResolveExistingPath(string relativeOrAbsolutePath)
    {
        var contentRootCandidate = ResolvePath(relativeOrAbsolutePath);

        if (File.Exists(contentRootCandidate) || Directory.Exists(contentRootCandidate))
        {
            return contentRootCandidate;
        }

        if (!_useBaseDirectoryFallback || Path.IsPathRooted(relativeOrAbsolutePath))
        {
            return contentRootCandidate;
        }

        var normalizedRelativePath = relativeOrAbsolutePath.Replace('/', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, normalizedRelativePath));
    }

    private static T ReadRequiredJsonFile<T>(string path, string errorPrefix)
    {
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"{errorPrefix}: archivo no encontrado en '{path}'.");
        }

        using var stream = File.OpenRead(path);
        var model = JsonSerializer.Deserialize<T>(stream, JsonOptions);

        if (model is null)
        {
            throw new InvalidOperationException($"{errorPrefix}: contenido vacío o inválido en '{path}'.");
        }

        return model;
    }
}
