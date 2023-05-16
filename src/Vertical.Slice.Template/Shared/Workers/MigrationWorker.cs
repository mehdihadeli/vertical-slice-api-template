using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertical.Slice.Template.Shared.Data;

namespace Vertical.Slice.Template.Shared.Workers;

public class MigrationWorker : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MigrationWorker> _logger;

    public MigrationWorker(IServiceScopeFactory serviceScopeFactory, ILogger<MigrationWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration worker started.");

        using var serviceScope = _serviceScopeFactory.CreateScope();
        var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<CatalogsDbContext>();

        _logger.LogInformation("Updating catalog database...");

        await catalogDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Catalog database Updated");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration worker stopped.");

        return Task.CompletedTask;
    }
}
