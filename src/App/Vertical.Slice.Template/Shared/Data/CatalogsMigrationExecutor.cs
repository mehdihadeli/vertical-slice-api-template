using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Persistence;

namespace Vertical.Slice.Template.Shared.Data;

public class CatalogsMigrationExecutor(CatalogsDbContext catalogsDbContext, ILogger<CatalogsMigrationExecutor> logger)
    : IMigrationExecutor
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migration worker started");

        logger.LogInformation("Updating catalog database...");

        await catalogsDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Catalog database Updated");
    }
}
