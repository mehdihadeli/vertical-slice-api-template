using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Persistence;
using Shared.Observability;

namespace Shared.Core.Persistence;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
// Here we use `IHostedService` instead of `BackgroundService` because we want to have control for running async task in StartAsync method and wait for completion not running it in background like `BackgroundService` in its StartAsync
public class MigrationWorker(IMigrationManager migrationManager, ILogger<MigrationWorker> logger) : IHostedService
{
    internal static readonly string ActivitySourceName = "DbMigrations";
    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Migration operation");

        try
        {
            await migrationManager.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating");

            activity?.SetExceptionTags(ex);

            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
