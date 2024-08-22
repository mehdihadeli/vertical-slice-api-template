using AutoBogus;
using FluentAssertions;
using Vertical.Slice.Template.Products;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.ReadModel;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.UnitTests.Products;

public class ProductsMappingTests
{
    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void can_map_create_product_request_to_create_product()
    {
        var createProductRequest = AutoFaker.Generate<CreateProductRequest>();
        var createProduct = createProductRequest.ToCreateProduct();

        // https://fluentassertions.com/objectgraphs/
        createProduct.Should().BeEquivalentTo(createProductRequest);
    }

    [Fact]
    [CategoryTrait(TestCategory.Unit)]
    public void can_map_product_read_to_product_dto()
    {
        var productRead = AutoFaker.Generate<ProductReadModel>();
        var productDto = productRead.ToProductDto();

        // https://fluentassertions.com/objectgraphs/
        productDto.Should().BeEquivalentTo(productRead);
    }
}
