using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.EndToEndTests;

[Collection(EndToEndTestCollection.Name)]
public class CatalogsEndToEndTestBase : EndToEndTestTestBase<CatalogsApiMetadata, CatalogsDbContext>
{
    public CatalogsEndToEndTestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }
}
