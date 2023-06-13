using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.Slice.Template.Shared.Abstractions.Ef;

namespace Vertical.Slice.Template.Shared.Workers;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services
public class SeedWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SeedWorker> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SeedWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SeedWorker> logger,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Seed worker started");
        if (!_webHostEnvironment.IsEnvironment("test"))
        {
            // https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate
            // https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

            foreach (var seeder in seeders.OrderBy(x => x.Order))
            {
                _logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                _logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seed worker stopped");

        return base.StopAsync(cancellationToken);
    }
}
