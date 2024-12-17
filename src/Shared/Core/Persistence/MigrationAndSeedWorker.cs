using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;
using Shared.Web.Extensions;

namespace Shared.Core.Persistence;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
// Hint: we can't guarantee execution order of our seeder, and because our migration should apply first we should apply migration before running all background services with our MigrationManager and before `app.RunAsync()` for running host and workers
public class MigrationAndSeedWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MigrationAndSeedWorker> logger,
    IWebHostEnvironment webHostEnvironment
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();

        await ApplyMigrations(serviceScope);

        await ApplySeeds(serviceScope, cancellationToken);
    }

    private async Task ApplySeeds(IServiceScope serviceScope, CancellationToken cancellationToken)
    {
        if (!webHostEnvironment.IsTest())
        {
            logger.LogInformation("Seed worker started");

            // https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate
            // https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
            var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

            foreach (var seeder in seeders.OrderBy(x => x.Order))
            {
                logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
                await seeder.SeedAllAsync(cancellationToken);
                logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
            }
        }
    }

    private static async Task ApplyMigrations(IServiceScope serviceScope)
    {
        var migrationManager = serviceScope.ServiceProvider.GetRequiredService<IMigrationManager>();
        await migrationManager.ExecuteAsync(CancellationToken.None);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (!webHostEnvironment.IsTest())
        {
            logger.LogInformation("Seed worker stopped");
        }

        return base.StopAsync(cancellationToken);
    }
}
