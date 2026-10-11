using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.EndToEndTests;

[CollectionDefinition(Name)]
public class EndToEndTestCollection
    : ICollectionFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    public const string Name = "End-To-End Test";
}
