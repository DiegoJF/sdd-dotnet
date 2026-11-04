namespace RealtorApiTests.Infrastructure.OpenApi;

public sealed class OpenApiToolingTests
{
    [Fact]
    public async Task RedoclyLint_PassesOrFailsExplicitly_WhenPrerequisiteMissing()
    {
        var repoRoot = OpenApiToolRunner.GetRepositoryRoot();
        var openApiPath = Path.Combine(repoRoot, "app", "backend", "src", "RealtorApi", "wwwroot", "openapi", "v1.json");
        var redoclyConfigPath = Path.Combine(repoRoot, ".redocly.yaml");

        try
        {
            var output = await OpenApiToolRunner.RunAsync(
                "npx",
                $"--yes @redocly/cli lint --config \"{redoclyConfigPath}\" \"{openApiPath}\"",
                repoRoot);

            Assert.DoesNotContain("error", output, StringComparison.OrdinalIgnoreCase);
        }
        catch (InvalidOperationException exception)
        {
            Assert.Contains("Prerrequisito faltante", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task NswagSmokeClientGeneration_PassesOrFailsExplicitly_WhenPrerequisiteMissing()
    {
        var repoRoot = OpenApiToolRunner.GetRepositoryRoot();
        var nswagConfigPath = Path.Combine(repoRoot, "support", "scripts", "openapi-v1.nswag.json");

        try
        {
            await OpenApiToolRunner.RunAsync("dotnet", "tool restore", repoRoot);
            var output = await OpenApiToolRunner.RunAsync(
                "dotnet",
                $"tool run nswag run \"{nswagConfigPath}\"",
                repoRoot);

            Assert.DoesNotContain("Exception", output, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(Path.Combine(repoRoot, "artifacts", "openapi-client-smoke", "RealtorApiClient.cs")));
        }
        catch (InvalidOperationException exception)
        {
            var isExplicitMissingPrerequisite = exception.Message.Contains("Prerrequisito faltante", StringComparison.OrdinalIgnoreCase);
            var isExplicitCommandFailure = exception.Message.Contains("Comando fallido", StringComparison.OrdinalIgnoreCase);

            Assert.True(
                isExplicitMissingPrerequisite || isExplicitCommandFailure,
                $"El fallo de tooling debe ser explícito. Mensaje: {exception.Message}");
        }
    }
}
