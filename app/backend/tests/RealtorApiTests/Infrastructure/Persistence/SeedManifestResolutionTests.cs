using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class SeedManifestResolutionTests
{
    [Fact]
    public void SeedManifestLoader_ResuelveRutasYLeeManifest()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest();
        workspace.WriteStatuses(new { value = "Available", description = "Disponible" });
        workspace.WriteProperties();

        var loader = new SeedManifestLoader(workspace.RootPath);
        var manifest = loader.LoadManifest();

        Assert.Equal("support/seed-data/properties-statuses.json", manifest.StatusesFile);
        Assert.Equal("support/seed-data/properties.json", manifest.PropertiesFile);

        var statusesPath = loader.ResolvePath(manifest.StatusesFile);
        var propertiesPath = loader.ResolvePath(manifest.PropertiesFile);

        Assert.True(File.Exists(statusesPath));
        Assert.True(File.Exists(propertiesPath));
    }
}
