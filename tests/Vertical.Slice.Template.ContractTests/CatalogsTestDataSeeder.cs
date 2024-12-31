using Shared.Abstractions.Persistence;
using Shared.Abstractions.Persistence.Ef;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;

namespace Vertical.Slice.Template.ContractTests;

public class CatalogsTestDataSeeder(CatalogsDbContext catalogsDbContext) : ITestDataSeeder
{
    public async Task SeedAllAsync(CancellationToken cancellationToken)
    {
        await SeedProducts(cancellationToken);
    }

    private async Task SeedProducts(CancellationToken cancellationToken)
    {
        IEnumerable<Product> productsFake = new ProductFake().Generate(2);
        await catalogsDbContext.Products.AddRangeAsync(productsFake, cancellationToken);
        await catalogsDbContext.SaveChangesAsync(cancellationToken);
    }

    public int Order => 1;
}
