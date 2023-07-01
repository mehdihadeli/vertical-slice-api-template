using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.ApiClientTests;

// https://stackoverflow.com/questions/43082094/use-multiple-collectionfixture-on-my-test-class-in-xunit-2-x
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
