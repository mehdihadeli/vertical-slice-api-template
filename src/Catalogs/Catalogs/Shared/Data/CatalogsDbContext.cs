using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Shared.Data;

public class CatalogsDbContext : DbContext
{
    public CatalogsDbContext(DbContextOptions<CatalogsDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
