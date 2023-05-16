using Vertical.Slice.Template.Shared.EF;

namespace Vertical.Slice.Template.Shared.Data;

public class CatalogsDbContextDesignFactory : DbContextDesignFactoryBase<CatalogsDbContext>
{
    public CatalogsDbContextDesignFactory()
        : base($"{nameof(PostgresOptions)}:{nameof(PostgresOptions.ConnectionString)}") { }
}
