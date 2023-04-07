using AutoBogus;
using Catalogs.Products.Features.CreatingProduct.v1;
using Catalogs.UnitTests.Common;
using FluentAssertions;
using NSubstitute;
using Tests.Shared.XunitCategories;

namespace Catalogs.UnitTests.Products.Features.CreatingProduct.v1;

[Collection(UnitTestCollection.Name)]
public class CreateProductTests : CatalogsUnitTestBase
{
    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task can_create_product_with_valid_inputs()
    {
        var createProductMock = new AutoFaker<CreateProduct>().Generate();

        var executor = Substitute.For<DbExecuters.CreateAndSaveProductExecutor>();
        executor
            .Invoke(Arg.Is<Product>(x => x.Id == createProductMock.Id), Arg.Any<CancellationToken>())
            .Returns(new ValueTask());

        // Arrange
        var handler = new CreateProductHandler(executor, GetFakeValidator<CreateProduct>(), Mapper);

        // Act
        var createdCustomerResponse = await handler.Handle(createProductMock, CancellationToken.None);

        //Assert
        await executor
            .Received(1)
            .Invoke(Arg.Is<Product>(x => x.Id == createProductMock.Id), Arg.Any<CancellationToken>());
        createdCustomerResponse.Should().NotBeNull();
        createdCustomerResponse.Id.Should().Be(createProductMock.Id);
    }
}
