using AutoBogus;
using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Clients.Catalogs.Dtos;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests.Catalogs;

public class CatalogsServiceCatalogsTests : CatalogsTestBase
{
    public CatalogsServiceCatalogsTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task create_product_can_create_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductClientDto>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .Generate();

        // Act
        var response = await CatalogsClient.CreateProductAsync(createProduct, CancellationToken.None);

        // Assert
        response.Should().NotBeEmpty();
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_id_can_get_product_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductClientDto>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .Generate();
        var id = await CatalogsClient.CreateProductAsync(createProduct, CancellationToken.None);

        // Act
        var response = await CatalogsClient.GetProductByIdAsync(id, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_product_by_page_can_get_products_with_valid_data()
    {
        // Arrange
        var createProduct = new AutoFaker<CreateProductClientDto>()
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .Generate();
        var id = await CatalogsClient.CreateProductAsync(createProduct, CancellationToken.None);

        // Act
        var response = await CatalogsClient.GetProductByPageAsync(
            new GetGetProductsByPageClientDto { PageSize = 10, PageNumber = 1 },
            CancellationToken.None
        );
        // Assert
        response.Should().NotBeNull();
        response.Items.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        response.Items.First().Id.Should().Be(id);
        response.PageNumber.Should().Be(1);
        response.TotalPages.Should().Be(1);
        response.PageSize.Should().Be(10);
    }
}
