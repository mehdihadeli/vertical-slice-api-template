using AutoBogus;
using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Clients.Dtos;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ConnectedServiceClientsTests.ConnectedServiceClient;

public class CatalogConnectedServiceClientTests(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : CatalogsTestBase(sharedFixture, outputHelper)
{
    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task create_product_can_create_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductClientRequestDto>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .Generate();

        // Act
        var createdProductId = await CatalogsConnectedServiceClient.CreateProductAsync(
            createProduct,
            CancellationToken.None
        );

        // Assert
        createdProductId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_id_can_get_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductClientRequestDto>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .Generate();
        var id = await CatalogsConnectedServiceClient.CreateProductAsync(createProduct, CancellationToken.None);

        // Act
        var product = await CatalogsConnectedServiceClient.GetProductByIdAsync(id, CancellationToken.None);

        // Assert
        product.Should().NotBeNull();
        product.Description.Should().NotBeNull();
        product.Name.Should().NotBeNull();
        product.Id.Should().NotBeEmpty();
        product.CategoryId.Should().NotBeEmpty();
        product.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_page_can_get_products_with_valid_data()
    {
        // Act
        var productsPageList = await CatalogsConnectedServiceClient.GetProductByPageAsync(
            new GetProductsByPageClientRequestDto { PageSize = 10, PageNumber = 1 },
            CancellationToken.None
        );

        // Assert
        productsPageList.Should().NotBeNull();
        productsPageList.Items.Should().NotBeNull();
        productsPageList.TotalCount.Should().Be(2);
        productsPageList.TotalPages.Should().Be(1);
        productsPageList.PageSize.Should().Be(10);
        productsPageList.PageNumber.Should().Be(1);

        var firstProduct = productsPageList.Items.ToList().FirstOrDefault();
        firstProduct.Should().NotBeNull();
        firstProduct!.Name.Should().NotBeNull();
        firstProduct.Id.Should().NotBeEmpty();
        firstProduct.CategoryId.Should().NotBeEmpty();
        firstProduct.Price.Should().BeGreaterThan(0);
        firstProduct.Description.Should().NotBeNull();
    }
}
