using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.TestsShared.TestBase;

public class EndToEndTestTest<TEntryPoint>(SharedFixture<TEntryPoint> sharedFixture, ITestOutputHelper outputHelper)
    : IntegrationTest<TEntryPoint>(sharedFixture, outputHelper)
    where TEntryPoint : class;

public abstract class EndToEndTestTestBase<TEntryPoint, TContext>(
    SharedFixtureWithEfCore<TEntryPoint, TContext> sharedFixture,
    ITestOutputHelper outputHelper
) : EndToEndTestTest<TEntryPoint>(sharedFixture, outputHelper)
    where TEntryPoint : class
    where TContext : DbContext
{
    public new SharedFixtureWithEfCore<TEntryPoint, TContext> SharedFixture { get; } = sharedFixture;
}
