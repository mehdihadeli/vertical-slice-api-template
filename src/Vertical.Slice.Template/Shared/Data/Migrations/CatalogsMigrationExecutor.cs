using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vertical.Slice.Template.Shared.Abstractions.Ef;

namespace Vertical.Slice.Template.Shared.Data.Migrations;

public class CatalogsMigrationExecutor : IMigrationExecutor
{
    private readonly CatalogsDbContext _catalogsDbContext;
    private readonly ILogger<CatalogsMigrationExecutor> _logger;

    public CatalogsMigrationExecutor(CatalogsDbContext catalogsDbContext, ILogger<CatalogsMigrationExecutor> logger)
    {
        _catalogsDbContext = catalogsDbContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migration worker started");

        _logger.LogInformation("Updating catalog database...");

        await _catalogsDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Catalog database Updated");
    }
}
