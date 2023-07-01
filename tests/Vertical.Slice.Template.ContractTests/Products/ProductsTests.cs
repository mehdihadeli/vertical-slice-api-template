using AutoBogus;
using Catalogs.ApiClient;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests.Products;

public class ProductsTests : TestBase
{
    public ProductsTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_create_product()
    {
        // we should not dispose this guestClient, because we reuse it in our tests
        var baseClient = SharedFixture.GuestClient;
        var client = new CatalogsApiClient(baseClient);

        var req = new AutoFaker<CreateProductRequest>().Generate();
        var createProductRequest = new CreateProductRequest(
            req.CategoryId,
            req.Description,
            req.Description,
            req.Price
        );

        var response = await client.CreateProductAsync(createProductRequest);
        response.Id.Should().NotBeEmpty();
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_product_by_id()
    {
        // we should not dispose this guestClient, because we reuse it in our tests
        var baseClient = SharedFixture.GuestClient;
        var client = new CatalogsApiClient(baseClient);
        var createProductResponse = await CreateProduct(client);

        // Act
        var productByIdResponse = await client.GetProductByIdAsync(createProductResponse.Id);

        // Assert
        productByIdResponse.Should().NotBeNull();
        productByIdResponse.Product.Should().NotBeNull();
        productByIdResponse.Product.Description.Should().NotBeNull();
        productByIdResponse.Product.Name.Should().NotBeNull();
        productByIdResponse.Product.Id.Should().NotBeEmpty();
        productByIdResponse.Product.CategoryId.Should().NotBeEmpty();
        productByIdResponse.Product.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_products_by_page()
    {
        // we should not dispose this guestClient, because we reuse it in our tests
        var baseClient = SharedFixture.GuestClient;
        var client = new CatalogsApiClient(baseClient);
        var createdProduct = await CreateProduct(client);

        var response = await client.GetProductsByPageAsync(pageSize: 10, pageNumber: 1, filters: null, sortOrder: null);
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull();
        response.Products.Items.Should().NotBeNull();
        response.Products.TotalCount.Should().Be(1);
        response.Products.TotalPages.Should().Be(1);
        response.Products.PageSize.Should().Be(10);
        response.Products.PageNumber.Should().Be(1);
        response.Products.Items.First().Should().NotBeNull();
        response.Products.Items.First().Description.Should().NotBeNull();
        response.Products.Items.First().Name.Should().NotBeNull();
        response.Products.Items.First().Id.Should().NotBeEmpty();
        response.Products.Items.First().CategoryId.Should().NotBeEmpty();
        //response.Products.Items.First().Price.Should().BeGreaterThan(0);
    }

    async Task<CreateProductResponse> CreateProduct(ICatalogsApiClient catalogsApiClient)
    {
        var req = new AutoFaker<CreateProductRequest>()
            .RuleFor(x => x.Price, f => f.Random.Double(10, 1000))
            .Generate();
        var createProductRequest = new CreateProductRequest(
            req.CategoryId,
            req.Description,
            req.Description,
            req.Price
        );
        var createProductResponse = await catalogsApiClient.CreateProductAsync(createProductRequest);
        return createProductResponse;
    }
}
