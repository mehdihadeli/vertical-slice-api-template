using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Persistence;

namespace Shared.Core.Persistence;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
// Here we use `IHostedService` instead of `BackgroundService` because we want to have control for running async task in StartAsync method and wait for completion not running it in background like `BackgroundService` in its StartAsync
public class MigrationWorker(IMigrationManager migrationManager) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await migrationManager.ExecuteAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
