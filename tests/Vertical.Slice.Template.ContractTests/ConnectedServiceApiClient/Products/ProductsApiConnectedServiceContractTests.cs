using AutoBogus;
using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.ContractTests.ConnectedServiceApiClient;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;
using CreateProductRequest = Vertical.Slice.Template.ContractTests.ConnectedServiceApiClient.CreateProductRequest;
using CreateProductResponse = Vertical.Slice.Template.ContractTests.ConnectedServiceApiClient.CreateProductResponse;

namespace Vertical.Slice.Template.ContractTests.Products;

// if we have any breaking change in the target openapi and affects on our current used schema contract should fail

public class ProductsApiConnectedServiceContractTests : CatalogsIntegrationTestBase
{
    private readonly ContractTestCatalogsConnectedServiceApiClient _contractTestClient;

    public ProductsApiConnectedServiceContractTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        // we should not dispose this guestClient, because we reuse it in our tests
        var baseClient = SharedFixture.GuestClient;
        _contractTestClient = new ContractTestCatalogsConnectedServiceApiClient(baseClient);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_create_product()
    {
        var req = new AutoFaker<CreateProductRequest>().Generate();
        var createProductRequest = new CreateProductRequest(
            req.CategoryId,
            req.Description,
            req.Description,
            req.Price
        );

        var response = await _contractTestClient.CreateProductAsync(createProductRequest);
        response.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_product_by_id()
    {
        var createProductResponse = await CreateTestProduct(_contractTestClient);

        // Act
        var productByIdResponse = await _contractTestClient.GetProductByIdAsync(createProductResponse.Id);

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
        var response = await _contractTestClient.GetProductsByPageAsync(
            pageSize: 10,
            pageNumber: 1,
            filters: null,
            sortOrder: null
        );
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull();
        response.Products.Items.Should().NotBeNull();
        response.Products.TotalCount.Should().Be(2);
        response.Products.TotalPages.Should().Be(1);
        response.Products.PageSize.Should().Be(10);
        response.Products.PageNumber.Should().Be(1);

        var firstProduct = response.Products.Items.FirstOrDefault();
        firstProduct.Should().NotBeNull();
        firstProduct!.Description.Should().NotBeNull();
        firstProduct.Name.Should().NotBeNull();
        firstProduct.Id.Should().NotBeEmpty();
        firstProduct.CategoryId.Should().NotBeEmpty();
        firstProduct.Price.Should().BeGreaterThan(0);
    }

    private static async Task<CreateProductResponse> CreateTestProduct(
        IContractTestCatalogsConnectedServiceApiClient catalogsApiClient
    )
    {
        var createProductRequest = new AutoFaker<CreateProductRequest>()
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.CategoryId, f => Guid.NewGuid())
            .RuleFor(x => x.Price, f => f.Random.Double(10, 1000))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .Generate();

        var createProductResponse = await catalogsApiClient.CreateProductAsync(createProductRequest);
        return createProductResponse;
    }
}
