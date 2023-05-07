using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.IntegrationTests.Products.Features.GettingProductByPage.v1;

public class GetProductsByPageTests : CatalogsIntegrationTestBase
{
    public GetProductsByPageTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    internal async Task can_get_existing_products_list_from_db()
    {
        // Arrange
        var fakeProducts = new ProductFake().Generate(3);
        await SharedFixture.InsertEfDbContextAsync(fakeProducts.ToArray());

        // Act
        var query = new GetProductsByPage();
        var listResult = (await SharedFixture.SendAsync(query)).Products;

        // Assert
        listResult.Should().NotBeNull();
        listResult.Items.Should().NotBeEmpty();
        listResult.Items.Should().HaveCount(3);
        listResult.PageNumber.Should().Be(1);
        listResult.PageSize.Should().Be(10);
        listResult.TotalCount.Should().Be(3);

        listResult.Items.Should().BeEquivalentTo(fakeProducts, options => options.ExcludingMissingMembers());
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    internal async Task can_get_existing_products_list_with_correct_page_size_and_page()
    {
        // Arrange
        var fakeProducts = new ProductFake().Generate(3);
        await SharedFixture.InsertEfDbContextAsync(fakeProducts.ToArray());

        // Act
        var query = new GetProductsByPage { PageNumber = 1, PageSize = 2 };
        var listResult = (await SharedFixture.SendAsync(query)).Products;

        // Assert
        listResult.Should().NotBeNull();
        listResult.Items.Should().NotBeEmpty();
        listResult.Items.Should().HaveCount(2);
        listResult.PageNumber.Should().Be(1);
        listResult.PageSize.Should().Be(2);
        listResult.TotalCount.Should().Be(3);
    }
}
