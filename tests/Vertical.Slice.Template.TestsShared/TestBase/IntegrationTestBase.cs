using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.TestsShared.TestBase;

public abstract class IntegrationTest<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly ITestOutputHelper _outputHelper;
    private IServiceScope? _scope;
    protected CancellationToken CancellationToken => CancellationTokenSource.Token;
    protected CancellationTokenSource CancellationTokenSource { get; }
    protected int Timeout => 180;
    protected IServiceScope Scope => _scope ??= SharedFixture.ServiceProvider.CreateScope(); // Build Service Provider here
    protected SharedFixture<TEntryPoint> SharedFixture { get; }

    protected IntegrationTest(SharedFixture<TEntryPoint> sharedFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        SharedFixture = sharedFixture;

        CancellationTokenSource = new(TimeSpan.FromSeconds(Timeout));
        CancellationToken.ThrowIfCancellationRequested();
    }

    // we use IAsyncLifetime in xunit instead of constructor when we have async operation
    public virtual async Task InitializeAsync() { }

    public virtual async Task DisposeAsync()
    {
        // it is better messages delete first
        await SharedFixture.ResetDatabasesAsync(CancellationToken);

        CancellationTokenSource.Cancel();

        Scope.Dispose();
    }
}

public abstract class IntegrationTestBase<TEntryPoint, TContext> : IntegrationTest<TEntryPoint>
    where TEntryPoint : class
    where TContext : DbContext
{
    protected IntegrationTestBase(
        SharedFixtureWithEfCore<TEntryPoint, TContext> sharedFixture,
        ITestOutputHelper outputHelper
    )
        : base(sharedFixture, outputHelper)
    {
        SharedFixture = sharedFixture;
    }

    public new SharedFixtureWithEfCore<TEntryPoint, TContext> SharedFixture { get; }
}
