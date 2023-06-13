using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.Shared.EF;

namespace Vertical.Slice.Template.Products.Data.Configurations;

public class ProductEntityTypeConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(Product).Pluralize().Underscore(), CatalogsDbContext.DefaultSchema);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(EfConstants.Lenght.Normal).IsRequired();

        builder.Property(p => p.Price).HasColumnType(EfConstants.ColumnTypes.PriceDecimal).IsRequired();

        builder.Property(p => p.Description).HasMaxLength(EfConstants.Lenght.Long);
    }
}
