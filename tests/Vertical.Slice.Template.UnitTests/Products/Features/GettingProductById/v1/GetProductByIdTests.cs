using AutoBogus;
using FluentAssertions;
using NSubstitute;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.ReadModel;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Vertical.Slice.Template.UnitTests.Common;

namespace Vertical.Slice.Template.UnitTests.Products.Features.GettingProductById.v1;

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

        var handler = new GetProductByIdHandler(executor, Mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        await executor.Received(1).Invoke(Arg.Is(product.Id), Arg.Any<CancellationToken>());
        result.Should().NotBeNull();
        result.Product.Id.Should().Be(product.Id);
        result.Product.Name.Should().Be(product.Name);
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task must_throw_not_found_exception_when_product_with_given_id_not_exist()
    {
        Guid notExistsProductId = Guid.NewGuid();
        var executor = Substitute.For<DbExecutors.GetProductByIdExecutor>();
        executor
            .Invoke(Arg.Is(notExistsProductId), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProductReadModel?>(null));

        var handler = new GetProductByIdHandler(executor, Mapper);
        var notExistProductQuery = new GetProductById(notExistsProductId);

        // Act
        Func<Task<GetProductByIdResult>> act = () => handler.Handle(notExistProductQuery, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        await executor.Received(1).Invoke(Arg.Is(notExistsProductId), Arg.Any<CancellationToken>());
    }

    [CategoryTrait(TestCategory.Unit)]
    [Fact]
    public async Task must_throw_exception_when_input_is_null()
    {
        var product = new AutoFaker<ProductReadModel>().Generate();
        var executor = Substitute.For<DbExecutors.GetProductByIdExecutor>();
        executor.Invoke(Arg.Is(product.Id), Arg.Any<CancellationToken>()).Returns(product);
        var handler = new GetProductByIdHandler(executor, Mapper);

        // Act
        Func<Task<GetProductByIdResult>> act = () => handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
        await executor.Received(0).Invoke(Arg.Is(product.Id), Arg.Any<CancellationToken>());
    }
}
