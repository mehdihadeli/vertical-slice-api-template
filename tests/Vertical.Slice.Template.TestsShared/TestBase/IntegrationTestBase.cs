using Meziantou.Extensions.Logging.InMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.Slice.Template.Shared.Abstractions.Ef;
using Vertical.Slice.Template.TestsShared.Fixtures;

namespace Vertical.Slice.Template.TestsShared.TestBase;

public abstract class IntegrationTest<TEntryPoint> :XunitContextBase, IAsyncLifetime
    where TEntryPoint : class
{
    private readonly ITestOutputHelper _outputHelper;
    protected CancellationToken CancellationToken => CancellationTokenSource.Token;
    protected CancellationTokenSource CancellationTokenSource { get; }
    protected int Timeout => 180;
    protected IServiceScope Scope { get; }
    protected SharedFixture<TEntryPoint> SharedFixture { get; }

    /// <summary>
    /// Use for tracking occured log events for testing purposes
    /// </summary>
    protected InMemoryLoggerProvider InMemoryLogTrackerProvider { get; }

    protected IntegrationTest(SharedFixture<TEntryPoint> sharedFixture, ITestOutputHelper outputHelper): base(outputHelper)
    {
        _outputHelper = outputHelper;
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

        // Build Service Provider here
        Scope = SharedFixture.ServiceProvider.CreateScope();
        InMemoryLogTrackerProvider = sharedFixture.Factory.InMemoryLogTrackerProvider;
    }

    protected virtual void RegisterTestConfigureServices(IServiceCollection services) { }

    protected virtual void RegisterTestAppConfigurations(
        IConfigurationBuilder builder,
        IConfiguration configuration,
        IHostEnvironment environment
    ) { }

    // we use IAsyncLifetime in xunit instead of constructor when we have async operation
    public virtual async Task InitializeAsync()
    {
        await RunSeedAndMigrationAsync();
    }

    private async Task RunSeedAndMigrationAsync()
    {
        var migrations = Scope.ServiceProvider.GetServices<IMigrationExecutor>();
        var seeders = Scope.ServiceProvider.GetServices<IDataSeeder>();

        if (!SharedFixture.AlreadyMigrated)
        {
            foreach (var migration in migrations)
            {
                SharedFixture.Logger.Information("Migration '{Migration}' started...", migrations.GetType().Name);
                await migration.ExecuteAsync(CancellationToken);
                SharedFixture.Logger.Information("Migration '{Migration}' ended...", migration.GetType().Name);
            }

            SharedFixture.AlreadyMigrated = true;
        }

        foreach (var seeder in seeders)
        {
            SharedFixture.Logger.Information("Seeder '{Seeder}' started...", seeder.GetType().Name);
            await seeder.SeedAllAsync(CancellationToken);
            SharedFixture.Logger.Information("Seeder '{Seeder}' ended...", seeder.GetType().Name);
        }
    }

    public virtual async Task DisposeAsync()
    {
        // it is better messages delete first
        await SharedFixture.ResetDatabasesAsync(CancellationToken);

        await CancellationTokenSource.CancelAsync();

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
