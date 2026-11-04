using System.Diagnostics;

namespace RealtorApiTests.Infrastructure.OpenApi;

public static class OpenApiToolRunner
{
    public static async Task<string> RunAsync(string fileName, string arguments, string workingDirectory, CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };

        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException($"No fue posible iniciar el proceso: {fileName} {arguments}");
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Prerrequisito faltante o no ejecutable: {fileName}. Detalle: {exception.Message}", exception);
        }

        var stdOut = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErr = await process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var output = (stdOut + Environment.NewLine + stdErr).Trim();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Comando fallido ({fileName} {arguments}). ExitCode={process.ExitCode}. Salida:{Environment.NewLine}{output}");
        }

        return output;
    }

    public static string GetRepositoryRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../../"));
    }
}
