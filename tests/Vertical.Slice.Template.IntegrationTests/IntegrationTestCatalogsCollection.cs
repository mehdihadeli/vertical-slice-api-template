using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.IntegrationTests;

[CollectionDefinition(Name)]
public class IntegrationTestCatalogsCollection
    : ICollectionFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    public const string Name = "Catalogs Integration Test";
}

[CollectionDefinition(Name)]
public class IntegrationTestUsersCollection : ICollectionFixture<SharedFixture<CatalogsApiMetadata>>
{
    public const string Name = "Users Integration Test";
}
