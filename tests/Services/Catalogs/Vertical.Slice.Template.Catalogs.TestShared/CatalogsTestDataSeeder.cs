using Shared.Abstractions.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;

namespace Vertical.Slice.Template.Catalogs.TestShared;

public sealed class CatalogsTestDataSeeder(CatalogsDbContext catalogsDbContext) : ITestDataSeeder
{
    public async Task SeedAllAsync(CancellationToken cancellationToken)
    {
        await catalogsDbContext.Products.ExecuteDeleteAsync(cancellationToken);
        IEnumerable<Product> products = new ProductFake().Generate(2);
        await catalogsDbContext.Products.AddRangeAsync(products, cancellationToken);
        await catalogsDbContext.SaveChangesAsync(cancellationToken);
    }

    public int Order => 1;
}
