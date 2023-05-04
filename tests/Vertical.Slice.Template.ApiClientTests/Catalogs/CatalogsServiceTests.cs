using ApiClient.Catalogs.Dtos;
using AutoBogus;
using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests.Catalogs;

public class CatalogsServiceTests : TestBase
{
    public CatalogsServiceTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task create_product_can_create_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductInput>().Generate();

        // Act
        var response = await CatalogsService.CreateProductAsync(createProduct, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_id_can_get_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductInput>().Generate();
        var createProductResponse = await CatalogsService.CreateProductAsync(createProduct, CancellationToken.None);

        // Act
        var response = await CatalogsService.GetProductByIdAsync(createProductResponse.Id, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Product.Id.Should().Be(createProductResponse.Id);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_page_can_get_products_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductInput>().Generate();
        var createProductResponse = await CatalogsService.CreateProductAsync(createProduct, CancellationToken.None);

        // Act
        var response = await CatalogsService.GetProductByPageAsync(
            new GetGetProductsByPageInput { PageSize = 10, PageNumber = 1 },
            CancellationToken.None
        );
        // Assert
        response.Should().NotBeNull();
        response.Products.Items.Should().NotBeNull();
        response.Products.Items.Should().HaveCount(1);
        response.Products.Items.First().Id.Should().Be(createProductResponse.Id);
        response.Products.PageNumber.Should().Be(1);
        response.Products.TotalPages.Should().Be(1);
        response.Products.PageSize.Should().Be(10);
    }
}
