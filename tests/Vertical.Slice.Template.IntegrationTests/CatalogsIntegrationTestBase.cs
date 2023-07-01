using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.IntegrationTests;

[Collection(IntegrationTestCatalogsCollection.Name)]
public class CatalogsIntegrationTestBase : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    public CatalogsIntegrationTestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }
}
