using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Humanizer;
using Shared.Core.Exceptions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Products.Features.CreatingProduct.v1;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;

namespace Vertical.Slice.Template.EndToEndTests.Products.Feature.CreatingProduct.v1;

public class CreateProductTests : CatalogsEndToEndTestBase
{
    public CreateProductTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        AssertionOptions.AssertEquivalencyUsing(options => options.ExcludingMissingMembers());
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task should_returns_created_status_code_using_valid_dto()
    {
        // Arrange
        var fakeCategoryId = Guid.NewGuid();

        var createProductRequestFake = new CreateProductRequestFake(fakeCategoryId).Generate();

        var route = Constants.Routes.Products.Create;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(route, createProductRequestFake);

        // Assert
        response.Should().Be201Created();
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task should_returns_valid_response_using_valid_dto()
    {
        // Arrange
        var fakeCategoryId = Guid.NewGuid();
        var createProductRequestFake = new CreateProductRequestFake(fakeCategoryId).Generate();
        var route = Constants.Routes.Products.Create;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(route, createProductRequestFake);

        // Assert
        response.Should().Satisfy<CreateProductResponse>(x => x.Id.Should().NotBeEmpty());
    }

    [Fact]
    [CategoryTrait(TestCategory.EndToEnd)]
    public async Task should_returns_validation_problem_using_invalid_dto()
    {
        // Arrange
        var request = new CreateProductRequest(string.Empty, Guid.Empty, 0, null);

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            Constants.Routes.Products.Create,
            request
        );

        // Assert
        response
            .Should()
            .Satisfy<ProblemDetails>(problem =>
            {
                problem.Title.Should().Be(nameof(ValidationException).Humanize(LetterCasing.Title));
                problem.Detail.Should().Contain("Name");
            })
            .And.Be400BadRequest();
    }
}
