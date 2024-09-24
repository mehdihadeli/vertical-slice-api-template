using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fakes.Products;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.IntegrationTests.Products.Features.CreatingProduct.v1;

public class CreateProductTests(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
) : CatalogsIntegrationTestBase(sharedFixture, outputHelper)
{
    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task should_create_new_product_with_valid_input_in_sql_db()
    {
        // Arrange
        var fakeCategoryId = Guid.NewGuid();

        var command = new CreateProductFake(fakeCategoryId).Generate();

        // Act
        var createdCustomerResponse = await SharedFixture.SendAsync(command, CancellationToken.None);

        // Assert
        createdCustomerResponse.Should().NotBeNull();
        createdCustomerResponse.Id.Should().NotBeEmpty();
        createdCustomerResponse.Id.Should().Be(command.Id);
    }
}
