using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.Shared.Data;
using Vertical.Slice.Template.TestsShared.Fixtures;
using Vertical.Slice.Template.TestsShared.TestBase;
using Xunit.Abstractions;

namespace Vertical.Slice.Template.ContractTests;

public class CatalogsIntegrationTestBase(
    SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext> sharedFixture,
    ITestOutputHelper outputHelper
)
    : IntegrationTestBase<CatalogsApiMetadata, CatalogsDbContext>(sharedFixture, outputHelper),
        IClassFixture<SharedFixtureWithEfCore<CatalogsApiMetadata, CatalogsDbContext>>;
