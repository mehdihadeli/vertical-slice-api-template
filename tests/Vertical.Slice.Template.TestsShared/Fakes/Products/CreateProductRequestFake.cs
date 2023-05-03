using AutoBogus;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;

namespace Vertical.Slice.Template.TestsShared.Fakes.Products;

public sealed class CreateProductRequestFake : AutoFaker<CreateProductRequest>
{
    public CreateProductRequestFake(Guid? categoryId = null)
    {
        RuleFor(x => x.CategoryId, _ => categoryId ?? Guid.NewGuid());
    }
}
