using System.Net.Http.Headers;
using AutoBogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Abstractions.Persistence;
using Shared.Core.Persistence;
using Shared.EF;
using Vertical.Slice.Template.TestsShared.Factory;
using WireMock.Server;
using Xunit.Sdk;
using ILogger = Serilog.ILogger;

namespace Vertical.Slice.Template.TestsShared.Fixtures;

public class SharedFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    // fields
    private readonly IMessageSink _messageSink;
    private IHttpContextAccessor? _httpContextAccessor;
    private IServiceProvider? _serviceProvider;
    private IConfiguration? _configuration;
    private HttpClient? _guestClient;

    // properties
    public WireMockServer WireMockServer { get; }
    public string WireMockServerUrl { get; }
    public event Func<Task>? SharedFixtureInitialized;
    public event Func<Task>? SharedFixtureDisposed;
    public ILogger Logger { get; }
    public PostgresContainerFixture PostgresContainerFixture { get; }
    public CustomWebApplicationFactory<TEntryPoint> Factory { get; set; }
    public IServiceProvider ServiceProvider => _serviceProvider ??= Factory.Services;

    public IConfiguration Configuration => _configuration ??= ServiceProvider.GetRequiredService<IConfiguration>();

    public IHttpContextAccessor HttpContextAccessor =>
        _httpContextAccessor ??= ServiceProvider.GetRequiredService<IHttpContextAccessor>();

    /// <summary>
    /// We should not dispose this GuestClient, because we reuse it in our tests
    /// </summary>
    public HttpClient GuestClient
    {
        get
        {
            if (_guestClient == null)
            {
                _guestClient = Factory.CreateDefaultClient();

                // Set the media type of the request to JSON - we need this for getting problem details result for all http calls because problem details just return response for request with media type JSON
                _guestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            return _guestClient;
        }
    }

    // constructor
    public SharedFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        messageSink.OnMessage(new DiagnosticMessage("Constructing SharedFixture..."));

        //https://github.com/trbenning/serilog-sinks-xunit
        Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestOutput(messageSink)
            .CreateLogger()
            .ForContext<SharedFixture<TEntryPoint>>();

        // //// TODO: Breaking change in the testcontainer upgrade
        // TestcontainersSettings.Logger = new Serilog.Extensions.Logging.SerilogLoggerFactory(Logger).CreateLogger(
        //     "TestContainer"
        // );

        // Service provider will build after getting with get accessors, we don't want to build our service provider here
        PostgresContainerFixture = new PostgresContainerFixture(messageSink);

        AutoFaker.Configure(b =>
        {
            // configure global AutoBogus settings here
            b.WithRecursiveDepth(3).WithTreeDepth(1).WithRepeatCount(1);
        });

        // close to equivalency required to reconcile precision differences between EF and Postgres
        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                .WhenTypeIs<DateTime>();

            options
                .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                .WhenTypeIs<DateTimeOffset>();

            return options;
        });

        // new WireMockServer() is equivalent to call WireMockServer.Start()
        WireMockServer = WireMockServer.Start();
        WireMockServerUrl = WireMockServer.Url!;

        Factory = new CustomWebApplicationFactory<TEntryPoint>();
    }

    public void SetOutputHelper(ITestOutputHelper outputHelper)
    {
        // var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
        // loggerFactory.AddXUnit(outputHelper);
        Factory.SetOutputHelper(outputHelper);
    }

    public async Task InitializeAsync()
    {
        // for having capability of overriding dependencies in IntegrationTestBase we should not build service provider here.
        _messageSink.OnMessage(new DiagnosticMessage("SharedFixture Started..."));

        // Service provider will build after getting with get accessors, we don't want to build our service provider here
        await PostgresContainerFixture.InitializeAsync();

        Factory.AddTestHostedService<MigrationWorker>();

        // with `AddOverrideEnvKeyValues` config changes are accessible during services registration
        Factory.AddOverrideEnvKeyValues(
            new Dictionary<string, string>
            {
                {
                    $"{nameof(PostgresOptions)}:{nameof(PostgresOptions.ConnectionString)}",
                    PostgresContainerFixture.Container.GetConnectionString()
                },
            }
        );

        // with `AddOverrideInMemoryConfig` config changes are accessible after services registration and build process
        Factory.AddOverrideInMemoryConfig(new Dictionary<string, string>());

        Factory.WithTestConfiguration(cfg =>
        {
            // Or we can override configuration explicitly, and it is accessible via IOptions<> and Configuration
            cfg["WireMockUrl"] = WireMockServerUrl;
        });

        if (SharedFixtureInitialized is not null)
        {
            await SharedFixtureInitialized.Invoke();
        }
    }

    public async Task DisposeAsync()
    {
        await PostgresContainerFixture.DisposeAsync();

        if (SharedFixtureDisposed is not null)
        {
            await SharedFixtureDisposed.Invoke();
        }
    }

    public void WithTestConfigureServices(Action<IServiceCollection> services)
    {
        Factory.WithTestConfigureServices(services);
    }

    public void WithTestConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> appConfiguration)
    {
        Factory.WithTestConfigureAppConfiguration(appConfiguration);
    }

    public async Task ResetDatabasesAsync(CancellationToken cancellationToken = default)
    {
        await PostgresContainerFixture.ResetDbAsync(cancellationToken);
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();

        var result = await action(scope.ServiceProvider);

        return result;
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteScopeAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return await mediator.Send(request, cancellationToken);
        });
    }

    // Ref: https://tech.energyhelpline.com/in-memory-testing-with-masstransit/
    public async ValueTask WaitUntilConditionMet(
        Func<Task<bool>> conditionToMet,
        int? timeoutSecond = null,
        string? exception = null
    )
    {
        var time = timeoutSecond ?? 300;

        var startTime = DateTime.Now;
        var timeoutExpired = false;
        var meet = await conditionToMet.Invoke();

        while (!meet)
        {
            if (timeoutExpired)
            {
                throw new TimeoutException(
                    exception ?? $"Condition not met for the test in the '{timeoutExpired}' second."
                );
            }

            await Task.Delay(100);
            meet = await conditionToMet.Invoke();
            timeoutExpired = DateTime.Now - startTime > TimeSpan.FromSeconds(time);
        }
    }
}
