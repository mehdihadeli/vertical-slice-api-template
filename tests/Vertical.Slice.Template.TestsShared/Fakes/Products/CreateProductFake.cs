using AutoBogus;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

namespace Vertical.Slice.Template.TestsShared.Fakes.Products;

public sealed class CreateProductFake : AutoFaker<CreateProduct>
{
    public CreateProductFake(Guid? categoryId = null)
    {
        RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.CategoryId, f => categoryId ?? f.Random.Guid())
            .RuleFor(x => x.Id, f => f.Random.Guid());
    }
}
