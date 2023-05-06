using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests;

public class TestBase
    : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>,
        IClassFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>
{
    public TestBase(
        SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper) { }
}
