using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Persistence;
using Shared.Core.Persistence;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.TestsShared.TestBase;

//https://bartwullems.blogspot.com/2019/09/xunit-async-lifetime.html
//https://www.danclarke.com/cleaner-tests-with-iasynclifetime
//https://xunit.net/docs/shared-context
public abstract class IntegrationTest<TEntryPoint> : XunitContextBase, IAsyncLifetime
    where TEntryPoint : class
{
    private IServiceScope? _serviceScope;
    private TestWorkersRunner _testWorkersRunner = default!;

    protected CancellationToken CancellationToken => CancellationTokenSource.Token;
    protected CancellationTokenSource CancellationTokenSource { get; }
    protected int Timeout => 180;

    // Build Service Provider here
    protected IServiceScope Scope => _serviceScope ??= SharedFixture.ServiceProvider.CreateScope();
    protected SharedFixture<TEntryPoint> SharedFixture { get; }

    protected IntegrationTest(SharedFixture<TEntryPoint> sharedFixture, ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        SharedFixture = sharedFixture;
        SharedFixture.SetOutputHelper(outputHelper);

        CancellationTokenSource = new(TimeSpan.FromSeconds(Timeout));
        CancellationToken.ThrowIfCancellationRequested();

        SharedFixture.ConfigureTestServices(RegisterTestConfigureServices);

        SharedFixture.ConfigureTestConfigureApp(
            (context, configurationBuilder) =>
            {
                RegisterTestAppConfigurations(configurationBuilder, context.Configuration, context.HostingEnvironment);
            }
        );
    }

    // we use IAsyncLifetime in xunit instead of constructor when we have async operation
    public virtual async Task InitializeAsync()
    {
        // for seeding, we should run it for each test separately here.
        var seederManager = SharedFixture.ServiceProvider.GetRequiredService<IDataSeederManager>();
        // DataSeedWorker is removed from dependency injection in the test so we can't resolve it directly.
        var seedWorker = new DataSeedWorker(seederManager);

        _testWorkersRunner = new([seedWorker]);
        await _testWorkersRunner.StartWorkersAsync(CancellationToken.None);
    }

    public virtual async Task DisposeAsync()
    {
        // it is better messages delete first
        await SharedFixture.ResetDatabasesAsync(CancellationToken);

        await CancellationTokenSource.CancelAsync();

        Scope.Dispose();
    }

    protected virtual void RegisterTestConfigureServices(IServiceCollection services) { }

    protected virtual void RegisterTestAppConfigurations(
        IConfigurationBuilder builder,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        //
    }
}

public abstract class IntegrationTestBase<TEntryPoint, TContext>(
    SharedFixtureWithEfCore<TEntryPoint, TContext> sharedFixture,
    ITestOutputHelper outputHelper
) : IntegrationTest<TEntryPoint>(sharedFixture, outputHelper)
    where TEntryPoint : class
    where TContext : DbContext
{
    public new SharedFixtureWithEfCore<TEntryPoint, TContext> SharedFixture { get; } = sharedFixture;
}
