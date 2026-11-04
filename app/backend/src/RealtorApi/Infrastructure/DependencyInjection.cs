using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;
using RealtorApi.Infrastructure.Slices;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Assembly assembly,
        IConfiguration configuration)
    {
        services.AddValidationConventions(assembly);
        services.RegisterSlices(assembly);
        services.RegisterHandlers(assembly);
        services.AddSingleton<SeedManifestLoader>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["Logging:ConnectionStrings:DefaultConnection"]
                ?? throw new InvalidOperationException(
                    "No se encontró ConnectionStrings:DefaultConnection para registrar AppDbContext.");

            var databaseProvider = configuration["Database:Provider"];

            var manifestLoader = serviceProvider.GetRequiredService<SeedManifestLoader>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var seederLogger = loggerFactory.CreateLogger("DatabaseSeeder");

            if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString);
            }

            options
                .UseSeeding((context, _) =>
                {
                    DatabaseSeeder.Seed((AppDbContext)context, manifestLoader, seederLogger);
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    await DatabaseSeeder.SeedAsync((AppDbContext)context, manifestLoader, seederLogger, cancellationToken);
                });
        });

        return services;
    }
}
