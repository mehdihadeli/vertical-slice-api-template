using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.IntegrationTests;

[CollectionDefinition(Name)]
public class IntegrationTestCollection
    : ICollectionFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    public const string Name = "Integration Test";
}
