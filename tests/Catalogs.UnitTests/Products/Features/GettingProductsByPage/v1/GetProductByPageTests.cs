using AutoBogus;
using Catalogs.Products.Features.GettingProductsByPage.v1;
using Catalogs.Products.ReadModel;
using Catalogs.UnitTests.Common;
using FluentAssertions;
using NSubstitute;
using Shared.Core.Types;
using Tests.Shared.XunitCategories;

namespace Catalogs.UnitTests.Products.Features.GettingProductsByPage.v1;

[Collection(UnitTestCollection.Name)]
public class GetProductByPageTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_get_products_with_valid_inputs()
    {
        // Arrange
        var productList = new AutoFaker<ProductReadModel>().Generate(5);

        var executor = Substitute.For<DbExecutors.GetProductsExecutor>();
        executor(Arg.Any<CancellationToken>()).Returns(productList.AsQueryable());
        var query = new GetProductsByPage() { PageSize = 10, PageNumber = 1 };

        var handler = new GetProductByPageHandler(executor, SieveProcessor, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        executor.Received(1).Invoke(Arg.Any<CancellationToken>());
        result.Should().NotBeNull();
    }
}
