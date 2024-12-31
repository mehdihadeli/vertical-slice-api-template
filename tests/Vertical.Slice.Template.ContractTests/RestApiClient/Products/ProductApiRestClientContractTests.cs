using System.Net.Http.Json;
using AutoBogus;
using FluentAssertions;
using Shared.Web.Extensions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.ContractTests.RestApiClient.Dtos;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests.RestApiClient.Products;

public class ProductApiRestClientContractTests(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : CatalogsIntegrationTestBase(sharedFixture, outputHelper)
{
    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_create_product()
    {
        var createProductRequest = new AutoFaker<ContractTestCreateProductClientRequestDto>()
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
            .Generate();
        var response = await SharedFixture.GuestClient.PostAsJsonAsync("api/v1/products", createProductRequest);

        await response.EnsureSuccessStatusCodeWithDetailAsync();
        var result = await response.Content.ReadFromJsonAsync<ContractTestCreateProductClientResponseDto>(
            cancellationToken: CancellationToken.None
        );

        result?.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_product_by_id()
    {
        var createProductResponse = await CreateTestProduct();

        // Act
        var productByIdResponse =
            await SharedFixture.GuestClient.GetFromJsonAsync<ContractTestGetProductByIdClientResponseDto>(
                $"api/v1/products/{createProductResponse.Id.ToString()}"
            );

        // Assert
        productByIdResponse.Should().NotBeNull();
        productByIdResponse!.Product.Should().NotBeNull();
        productByIdResponse.Product.Description.Should().NotBeNull();
        productByIdResponse.Product.Name.Should().NotBeNull();
        productByIdResponse.Product.Id.Should().NotBeEmpty();
        productByIdResponse.Product.CategoryId.Should().NotBeEmpty();
        productByIdResponse.Product.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task contracts_should_pass_for_get_products()
    {
        // Arrange
        var pageSize = 10;
        var pageNumber = 1;

        // Act
        var getProductByPageClientResponse =
            await SharedFixture.GuestClient.GetFromJsonAsync<ContractTestGetProductByPageClientResponseDto>(
                $"api/v1/products?limit={pageSize}&skip={pageNumber}",
                cancellationToken: CancellationToken.None
            );

        // Assert
        getProductByPageClientResponse.Should().NotBeNull();
        getProductByPageClientResponse!.Products.PageSize.Should().Be(pageSize);
        getProductByPageClientResponse!.Products.PageNumber.Should().Be(pageNumber);
        getProductByPageClientResponse.Products.Items.Should().NotBeNull();
        getProductByPageClientResponse.Products.Items.Should().HaveCountGreaterThan(0);
        var product = getProductByPageClientResponse.Products.Items.FirstOrDefault();
        product.Should().NotBeNull();
        product!.Id.Should().NotBe(Guid.Empty);
        product.Name.Should().NotBeNullOrEmpty();
        product.Description.Should().NotBeNullOrEmpty();
        product.Price.Should().BeGreaterThan(0);
        product.CategoryId.Should().NotBe(Guid.Empty);
    }

    private async Task<ContractTestCreateProductClientResponseDto> CreateTestProduct()
    {
        var createProductRequest = new AutoFaker<ContractTestCreateProductClientRequestDto>()
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.CategoryId, f => Guid.NewGuid())
            .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .Generate();

        var response = await SharedFixture.GuestClient.PostAsJsonAsync("api/v1/products", createProductRequest);

        await response.EnsureSuccessStatusCodeWithDetailAsync();
        var result = await response.Content.ReadFromJsonAsync<ContractTestCreateProductClientResponseDto>(
            cancellationToken: CancellationToken.None
        );

        return result!;
    }
}
