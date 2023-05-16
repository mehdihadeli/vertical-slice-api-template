using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.EF;

namespace Vertical.Slice.Template.Shared.Data;

public class CatalogsDbContext : EfDbContextBase
{
    public const string DefaultSchema = "catalog";

    public CatalogsDbContext(DbContextOptions<CatalogsDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
