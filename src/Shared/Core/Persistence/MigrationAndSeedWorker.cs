using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Persistence;
using Shared.Web.Extensions;

namespace Shared.Core.Persistence;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
// Hint: we can't guarantee execution order of our seeder, and because our migration should apply first we should apply migration before running all background services with our MigrationManager and before `app.RunAsync()` for running host and workers
public class MigrationAndSeedWorker(
    IDataSeederManager dataSeederManager,
    IMigrationManager migrationManager,
    IWebHostEnvironment webHostEnvironment,
    ILogger<MigrationAndSeedWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await ApplyMigrations();

        await ApplySeeds(cancellationToken);
    }

    private async Task ApplySeeds(CancellationToken cancellationToken)
    {
        await dataSeederManager.ExecuteAsync(cancellationToken);
    }

    private async Task ApplyMigrations()
    {
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
