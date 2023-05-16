using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Products.Features.GettingProductById.v1;
using Vertical.Slice.Template.Products.Models;
using Vertical.Slice.Template.Shared.Core.Exceptions;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Extensions;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.EndToEndTests.Products.Feature.GettingProductById;

public class GetProductByIdTests : CatalogsEndToEndTestBase
{
    public GetProductByIdTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task can_returns_ok_status_code_using_valid_id_and_auth_credentials()
    {
        // Arrange
        Product productFake = new ProductFake().Generate();
        await SharedFixture.InsertEfDbContextAsync(productFake);
        var route = Constants.Routes.Products.GetById(productFake.Id);

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(route);

        // Assert
        response.Should().Be200Ok();
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task can_returns_valid_response_using_valid_id_and_auth_credentials()
    {
        // Arrange
        Product productFake = new ProductFake().Generate();
        await SharedFixture.InsertEfDbContextAsync(productFake);
        var route = Constants.Routes.Products.GetById(productFake.Id);

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(route);

        // Assert
        response.Should().Satisfy<GetProductByIdResponse>(x => x.Product.Should().BeEquivalentTo(productFake));

        // OR
        // response
        //     .Should()
        //     .Satisfy(
        //         givenModelStructure: new { Product = new { Id = default(Guid), Name = default(string), } },
        //         assertion: model =>
        //         {
        //             model.Product.Id.Should().Be(productFake.Id);
        //             model.Product.Name.Should().Be(productFake.Name);
        //         }
        //     );

        // OR
        // response.Should().BeAs(new { Product = new { Id = productFake.Id, Name = productFake.Name, } });
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task must_returns_not_found_status_code_when_product_with_given_id_not_exists()
    {
        // Arrange
        var notExistsId = Guid.NewGuid();
        var route = Constants.Routes.Products.GetById(notExistsId);

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(route);

        // Assert
        response
            .Should()
            .Satisfy<ProblemDetails>(pr =>
            {
                pr.Detail.Should().Be($"product with id {notExistsId} not found");
                pr.Title.Should().Be(nameof(NotFoundException));
                pr.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
            })
            .And.Be404NotFound();

        // OR
        // response
        //     .Should()
        //     .HaveError("title", nameof(NotFoundException))
        //     .And.HaveError("detail", $"product with id {notExistsId} not found")
        //     .And.HaveError("type", "https://tools.ietf.org/html/rfc7231#section-6.5.4")
        //     .And.HaveErrorMessage($"product with id {notExistsId} not found")
        //     .And.Be404NotFound();
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task must_returns_bad_request_status_code_with_empty_id()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var route = Constants.Routes.Products.GetById(invalidId);

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(route);

        // Assert
        response
            .Should()
            .Satisfy<ProblemDetails>(pr =>
            {
                pr.Detail.Should().Contain("'Id' must not be empty.");
                pr.Title.Should().Be(nameof(ValidationException));
                pr.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");
            })
            .And.Be400BadRequest();

        // // OR
        // response
        //     .Should()
        //     .ContainsProblemDetail(
        //         new ProblemDetails
        //         {
        //             Detail = "'Id' must not be empty.",
        //             Title = nameof(ValidationException),
        //             Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        //         }
        //     )
        //     .And.Be400BadRequest();
    }
}
