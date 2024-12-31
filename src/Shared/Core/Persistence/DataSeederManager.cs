using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;
using Shared.Web.Extensions;

namespace Shared.Core.Persistence;

public class DataSeederManager(
    IWebHostEnvironment webHostEnvironment,
    ILogger<DataSeederManager> logger,
    IServiceProvider serviceProvider
) : IDataSeederManager
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        List<IDataSeeder> seeders;

        if (!webHostEnvironment.IsTest())
        {
            logger.LogInformation("Seeding application data started");

            // https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate
            // https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
            seeders = scope.ServiceProvider.GetServices<IDataSeeder>().Where(x => x is not ITestDataSeeder).ToList();
        }
        else
        {
            logger.LogInformation("Seeding test data started");
            seeders = scope.ServiceProvider.GetServices<IDataSeeder>().Where(x => x is ITestDataSeeder).ToList();
        }

        foreach (var seeder in seeders.OrderBy(x => x.Order))
        {
            await seeder.SeedAllAsync(cancellationToken);
            logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
        }

        logger.LogInformation("Seeding finished");
    }
}
