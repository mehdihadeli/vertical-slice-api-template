using AutoBogus;
using FluentAssertions;
using NSubstitute;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Products.Features.GettingProductsByPage.v1;

[Collection(UnitTestCollection.Name)]
public class GetProductByPageTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_get_products_with_valid_inputs()
    {
        // Arrange
        var productList = new AutoFaker<Product>().Generate(5);

        var executor = Substitute.For<DbExecutors.GetProductsExecutor>();
        executor(Arg.Any<CancellationToken>()).Returns(productList.AsQueryable());
        var query = new GetProductsByPage();

        var handler = new GetProductByPageHandler(executor, SieveProcessor, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        executor.Received(1).Invoke(Arg.Any<CancellationToken>());
        result.Should().NotBeNull();
    }
}
