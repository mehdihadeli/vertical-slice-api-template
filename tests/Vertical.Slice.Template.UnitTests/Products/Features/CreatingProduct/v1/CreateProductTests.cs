using AutoBogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Products.Features.CreatingProduct.v1;

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
        var handler = new CreateProductHandler(
            executor,
            Mapper,
            Substitute.For<IMediator>(),
            NullLogger<CreateProductHandler>.Instance
        );

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
