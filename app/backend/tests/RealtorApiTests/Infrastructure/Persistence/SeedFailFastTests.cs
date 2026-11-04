using RealtorApi.Infrastructure.Persistence;

namespace RealtorApiTests.Infrastructure.Persistence;

public sealed class SeedFailFastTests
{
    [Fact]
    public void SeedManifestLoader_FallaSiNoExisteManifestCritico()
    {
        using var workspace = new SeedTestWorkspace();
        var loader = new SeedManifestLoader(workspace.RootPath, useBaseDirectoryFallback: false);

        var action = () => loader.LoadManifest();

        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("seed-manifest.json", exception.Message);
    }

    [Fact]
    public void SeedManifestLoader_FallaSiFaltanJsonCriticos()
    {
        using var workspace = new SeedTestWorkspace();
        workspace.WriteManifest(
            statusesFile: "support/seed-data/missing-statuses.json",
            propertiesFile: "support/seed-data/missing-properties.json");

        var loader = new SeedManifestLoader(workspace.RootPath, useBaseDirectoryFallback: false);
        var manifest = loader.LoadManifest();

        var statusesAction = () => loader.LoadStatuses(manifest);
        var propertiesAction = () => loader.LoadProperties(manifest);

        Assert.Throws<InvalidOperationException>(statusesAction);
        Assert.Throws<InvalidOperationException>(propertiesAction);
    }
}
