using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RealtorApiTests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
	private readonly string _contentRootPath = Path.Combine(
		Path.GetTempPath(),
		"realtor-api-tests",
		Guid.NewGuid().ToString("N"));

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		Directory.CreateDirectory(_contentRootPath);

		var seedSource = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../../../support/seed-data"));
		var seedDestination = Path.Combine(_contentRootPath, "support", "seed-data");

		CopyDirectory(seedSource, seedDestination);
		Directory.CreateDirectory(Path.Combine(_contentRootPath, "wwwroot", "assets", "properties"));

		builder.UseSetting(WebHostDefaults.ContentRootKey, _contentRootPath);
		builder.UseEnvironment("Development");
		builder.ConfigureAppConfiguration((_, configurationBuilder) =>
		{
			configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["ConnectionStrings:DefaultConnection"] = $"Data Source={Path.Combine(_contentRootPath, "realtor-api-tests.db")}",
				["Database:Provider"] = "Sqlite"
			});
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		if (!disposing)
		{
			return;
		}

		if (!Directory.Exists(_contentRootPath))
		{
			return;
		}

		try
		{
			Directory.Delete(_contentRootPath, recursive: true);
		}
		catch
		{
			// Ignorado intencionalmente para no fallar tests por cleanup de archivos en uso.
		}
	}

	private static void CopyDirectory(string source, string destination)
	{
		Directory.CreateDirectory(destination);

		foreach (var file in Directory.GetFiles(source))
		{
			var destinationPath = Path.Combine(destination, Path.GetFileName(file));
			File.Copy(file, destinationPath, overwrite: true);
		}

		foreach (var directory in Directory.GetDirectories(source))
		{
			var childDestination = Path.Combine(destination, Path.GetFileName(directory));
			CopyDirectory(directory, childDestination);
		}
	}
}
