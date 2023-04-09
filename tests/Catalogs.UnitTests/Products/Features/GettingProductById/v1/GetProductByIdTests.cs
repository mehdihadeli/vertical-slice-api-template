using AutoBogus;
using Catalogs.Products.Features.GettingProductById.v1;
using Catalogs.Products.ReadModel;
using Catalogs.UnitTests.Common;
using FluentAssertions;
using NSubstitute;
using Tests.Shared.XunitCategories;

namespace Catalogs.UnitTests.Products.Features.GettingProductById.v1;

[Collection(UnitTestCollection.Name)]
public class GetProductByIdTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_get_product_with_given_id()
    {
        // Arrange
        var product = new AutoFaker<ProductReadModel>().Generate();
        var executor = Substitute.For<DbExecutors.GetProductByIdExecutor>();
        executor.Invoke(Arg.Is(product.Id), Arg.Any<CancellationToken>()).Returns(product);
        var query = new GetProductById(product.Id);

        // Act
        var handler = new GetProductByIdHandler(executor, Mapper);
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        await executor.Received(1).Invoke(Arg.Is(product.Id), Arg.Any<CancellationToken>());
        result.Should().NotBeNull();
        result.Product.Id.Should().Be(product.Id);
        result.Product.Name.Should().Be(product.Name);
    }
}
