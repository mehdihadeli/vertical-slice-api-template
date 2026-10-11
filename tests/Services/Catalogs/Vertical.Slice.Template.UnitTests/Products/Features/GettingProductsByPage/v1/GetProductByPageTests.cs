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
        var productList = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Name = "Alpha",
                Price = 10,
                Description = "First",
            },
            new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Name = "Beta",
                Price = 20,
                Description = "Second",
            },
        }.AsQueryable();

        var executor = Substitute.For<DbExecutors.GetProductsExecutor>();
        executor(Arg.Any<CancellationToken>()).Returns(productList);
        var query = new GetProductsByPage { PageNumber = 1, PageSize = 1 };

        var handler = new GetProductByPageHandler(executor, SieveProcessor);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        executor.Received(1).Invoke(Arg.Any<CancellationToken>());
        result.Products.PageNumber.Should().Be(1);
        result.Products.PageSize.Should().Be(1);
        result.Products.TotalCount.Should().Be(2);
        result.Products.Items.Should().ContainSingle();
        result.Products.Items.Single().Name.Should().Be("Alpha");
    }
}
