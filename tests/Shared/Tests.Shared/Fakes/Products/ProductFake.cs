using Bogus;
using Vertical.Slice.Template.Products.Models;

namespace Vertical.Slice.Template.TestsShared.Fakes.Products;

public sealed class ProductFake : Faker<Product>
{
    public ProductFake(IEnumerable<Guid>? categoryIds = null)
    {
        RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.CategoryId, f => categoryIds == null ? Guid.NewGuid() : f.PickRandom(categoryIds))
            .RuleFor(x => x.Id, f => f.Random.Guid());
    }
}
