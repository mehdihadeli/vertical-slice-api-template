using AutoBogus;
using FluentAssertions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests.KiotaApiClient.Products;

public class ProductsApiKiotaContractTests : CatalogsIntegrationTestBase
{
    private readonly ContractTestCatalogsKiotaApiClient _contractTestClient;

    public ProductsApiKiotaContractTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        // we should not dispose this guestClient, because we reuse it in our tests
        // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
        var authenticationProvider = new AnonymousAuthenticationProvider();

        var requestAdapter = new HttpClientRequestAdapter(
            authenticationProvider,
            httpClient: SharedFixture.GuestClient
        );
        _contractTestClient = new ContractTestCatalogsKiotaApiClient(requestAdapter);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_create_product()
    {
        var createProductRequest = new AutoFaker<Models.CreateProductRequest>()
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.CategoryId, f => Guid.NewGuid())
            .RuleFor(x => x.Price, f => f.Random.Double(10, 1000))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .Generate();

        // Act
        //        // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
        var createdProductResponse = await _contractTestClient.Api.V1.Products.PostAsync(createProductRequest);

        createdProductResponse.Should().NotBeNull();
        createdProductResponse?.Id.Should().NotBeNull();
        createdProductResponse!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_product_by_id()
    {
        var createProductResponse = await CreateTestProduct();

        // Act
        // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
        var productByIdResponse = await _contractTestClient.Api.V1.Products[createProductResponse.Id!.Value].GetAsync();

        // Assert
        productByIdResponse.Should().NotBeNull();
        productByIdResponse!.Product.Should().NotBeNull();
        productByIdResponse.Product!.Description.Should().NotBeNull();
        productByIdResponse.Product.Name.Should().NotBeNull();
        productByIdResponse.Product.Id.Should().NotBeEmpty();
        productByIdResponse.Product.CategoryId.Should().NotBeEmpty();
        productByIdResponse.Product.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_products_by_page()
    {
        // https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet#create-the-client-application
        var response = await _contractTestClient.Api.V1.Products.GetAsync(x =>
        {
            x.QueryParameters.PageNumber = 1;
            x.QueryParameters.PageSize = 10;
        });

        response.Should().NotBeNull();
        response!.Products.Should().NotBeNull();
        response.Products!.Items.Should().NotBeNull();
        response.Products.TotalCount.Should().Be(2);
        response.Products.TotalPages.Should().Be(1);
        response.Products.PageSize.Should().Be(10);
        response.Products.PageNumber.Should().Be(1);

        var firstProduct = response.Products.Items!.FirstOrDefault();
        firstProduct.Should().NotBeNull();
        firstProduct!.Description.Should().NotBeNull();
        firstProduct.Name.Should().NotBeNull();
        firstProduct.Id.Should().NotBeEmpty();
        firstProduct.CategoryId.Should().NotBeEmpty();
        firstProduct.Price.Should().BeGreaterThan(0);
    }

    private async Task<Models.CreateProductResponse> CreateTestProduct()
    {
        var createProductRequest = new AutoFaker<Models.CreateProductRequest>()
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.CategoryId, f => Guid.NewGuid())
            .RuleFor(x => x.Price, f => f.Random.Double(10, 1000))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .Generate();

        var createdProductResponse = await _contractTestClient.Api.V1.Products.PostAsync(createProductRequest);

        return createdProductResponse!;
    }
}
