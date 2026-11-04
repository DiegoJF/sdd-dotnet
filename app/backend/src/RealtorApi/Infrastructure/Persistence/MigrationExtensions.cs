using Microsoft.EntityFrameworkCore;

namespace RealtorApi.Infrastructure.Persistence;

public static class MigrationExtensions
{
    public static async Task MigrateAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var providerName = context.Database.ProviderName ?? string.Empty;

        if (providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            await context.Database.EnsureCreatedAsync();
            return;
        }

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
            return;
        }

        await context.Database.EnsureCreatedAsync();
    }
}
