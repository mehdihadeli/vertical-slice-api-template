using FluentAssertions;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.XunitCategories;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ApiClientTests.RickAndMorty;

public class RickAndMortyApiTests : TestBase
{
    public RickAndMortyApiTests(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }

    [Fact]
    [CategoryTrait(TestCategory.Integration)]
    public async Task get_character_by_id_can_get_character_with_valid_data()
    {
        // Arrange
        var id = 1;

        // Act
        var response = await RickAndMortyClient.GetCharacterById(id, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(id);
    }
}
