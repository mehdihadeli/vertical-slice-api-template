using FluentAssertions;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Products.Features.GettingProductsByPage.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.EndToEndTests.Products.Feature.GettingProductsByPage;

public class GetProductsByPageTests(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : CatalogsEndToEndTestBase(sharedFixture, outputHelper)
{
    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task should_returns_products_and_page_metadata_using_valid_page_request()
    {
        // Arrange
        var seededProducts = new[]
        {
            new ProductFake().RuleFor(product => product.Name, "Alpha").Generate(),
            new ProductFake().RuleFor(product => product.Name, "Beta").Generate(),
            new ProductFake().RuleFor(product => product.Name, "Gamma").Generate(),
        };
        await SharedFixture.InsertEfDbContextAsync(seededProducts);

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(
            $"{Constants.Routes.Products.GetByPage}?pageNumber=1&pageSize=2"
        );

        // Assert
        response.Should().Be200Ok();
        var body = await response.Content.ReadFromJsonAsync<JsonObject>();
        var products = body!["products"]!.AsObject();

        products["pageNumber"]!.GetValue<int>().Should().Be(1);
        products["pageSize"]!.GetValue<int>().Should().Be(2);
        products["totalCount"]!.GetValue<int>().Should().Be(3);
        products["items"]!.AsArray().Select(item => item!["name"]!.GetValue<string>())
            .Should().Equal("Alpha", "Beta");
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task should_returns_bad_request_using_invalid_page_request()
    {
        // Act
        var response = await SharedFixture.GuestClient.GetAsync(
            $"{Constants.Routes.Products.GetByPage}?pageNumber=0&pageSize=0"
        );

        // Assert
        response.Should().Be400BadRequest();
    }
}
