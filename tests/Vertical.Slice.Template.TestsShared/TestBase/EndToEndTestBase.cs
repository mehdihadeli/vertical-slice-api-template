using Microsoft.EntityFrameworkCore;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.TestsShared.TestBase;

public class EndToEndTestTest<TEntryPoint> : IntegrationTest<TEntryPoint>
    where TEntryPoint : class
{
    public EndToEndTestTest(SharedFixture<TEntryPoint> sharedFixture, ITestOutputHelper outputHelper)
        : base(sharedFixture, outputHelper) { }
}

public abstract class EndToEndTestTestBase<TEntryPoint, TContext> : EndToEndTestTest<TEntryPoint>
    where TEntryPoint : class
    where TContext : DbContext
{
    protected EndToEndTestTestBase(
        SharedFixtureWithEfCore<TEntryPoint, TContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        SharedFixture = sharedFixture;
    }

    public new SharedFixtureWithEfCore<TEntryPoint, TContext> SharedFixture { get; }
}
