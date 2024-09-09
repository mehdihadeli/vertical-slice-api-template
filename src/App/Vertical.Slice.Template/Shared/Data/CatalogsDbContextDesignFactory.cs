using Shared.EF;

namespace Vertical.Slice.Template.Shared.Data;

public class CatalogsDbContextDesignFactory()
    : DbContextDesignFactoryBase<CatalogsDbContext>(
        $"{nameof(PostgresOptions)}:{nameof(PostgresOptions.ConnectionString)}"
    );
