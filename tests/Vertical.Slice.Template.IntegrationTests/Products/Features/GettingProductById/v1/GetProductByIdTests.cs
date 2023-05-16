using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.IntegrationTests.Products.Features.GettingProductById.v1;

public class GetProductByIdTests : CatalogsIntegrationTestBase
{
    public GetProductByIdTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    internal async Task can_returns_valid_product_dto()
    {
        // Arrange
        Product productFake = new ProductFake().Generate();
        await SharedFixture.InsertEfDbContextAsync(productFake);

        // Act
        var query = new GetProductById(productFake.Id);
        var productDto = (await SharedFixture.SendAsync(query)).Product;

        // Assert
        productDto.Should().BeEquivalentTo(productFake, options => options.ExcludingMissingMembers());
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    internal async Task must_throw_not_found_exception_when_item_does_not_exists_in_postgres()
    {
        // Act
        var query = new GetProductById(Guid.NewGuid());
        Func<Task> act = async () => _ = await SharedFixture.SendAsync(query);

        // Assert
        //https://fluentassertions.com/exceptions/
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
