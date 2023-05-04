using AutoBogus;
using DotNet.Testcontainers.Configurations;
using FluentAssertions;
using FluentAssertions.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.EF;
using Vertical.Slice.Template.Api;
using Vertical.Slice.Template.TestsShared.Factory;
using Xunit.Sdk;

namespace Vertical.Slice.Template.TestsShared.Fixtures;

public class SharedFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly IMessageSink _messageSink;
    private IHttpContextAccessor? _httpContextAccessor;
    private IServiceProvider? _serviceProvider;
    private IConfiguration? _configuration;

    public Func<Task>? OnSharedFixtureInitialized;
    public Func<Task>? OnSharedFixtureDisposed;

    public ILogger Logger { get; }
    public MsSqlContainerFixture MsSqlContainerFixture { get; }
    public PostgresContainerFixture PostgresContainerFixture { get; }
    public WebApplicationFactory<CatalogsApiMetadata> Factory { get; set; }
    public IServiceProvider ServiceProvider => _serviceProvider ??= Factory.Services;

    public IConfiguration Configuration => _configuration ??= ServiceProvider.GetRequiredService<IConfiguration>();

    public IHttpContextAccessor HttpContextAccessor =>
        _httpContextAccessor ??= ServiceProvider.GetRequiredService<IHttpContextAccessor>();

    public SharedFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        messageSink.OnMessage(new DiagnosticMessage("Constructing SharedFixture..."));

        //https://github.com/trbenning/serilog-sinks-xunit
        Logger = new LoggerConfiguration().MinimumLevel
            .Verbose()
            .WriteTo.TestOutput(messageSink)
            .CreateLogger()
            .ForContext<SharedFixture<TEntryPoint>>();

        TestcontainersSettings.Logger = new Serilog.Extensions.Logging.SerilogLoggerFactory(Logger).CreateLogger(
            "TestContainer"
        );

        // Service provider will build after getting with get accessors, we don't want to build our service provider here
        MsSqlContainerFixture = new MsSqlContainerFixture(messageSink);
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

        Factory = new CustomWebApplicationFactory();
        Factory = Factory.WithWebHostBuilder(wb =>
        {
            wb.ConfigureAppConfiguration(
                (context, builder) =>
                {
                    var Dict = new Dictionary<string, string>
                    {
                        {
                            $"{nameof(PostgresOptions)}:{nameof(PostgresOptions.ConnectionString)}",
                            PostgresContainerFixture.Container.GetConnectionString()
                        }
                    };

                    // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
                    // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
                    builder.AddInMemoryCollection(Dict);
                }
            );
        });
    }

    public async Task InitializeAsync()
    {
        _messageSink.OnMessage(new DiagnosticMessage("SharedFixture Started..."));

        // Service provider will build after getting with get accessors, we don't want to build our service provider here
        //await MsSqlContainerFixture.InitializeAsync();
        await PostgresContainerFixture.InitializeAsync();

        var initCallback = OnSharedFixtureInitialized?.Invoke();
        if (initCallback != null)
        {
            await initCallback;
        }
    }

    public async Task DisposeAsync()
    {
        //await MsSqlContainerFixture.DisposeAsync();
        await PostgresContainerFixture.DisposeAsync();

        var disposeCallback = OnSharedFixtureDisposed?.Invoke();
        if (disposeCallback != null)
        {
            await disposeCallback;
        }

        await Factory.DisposeAsync();

        _messageSink.OnMessage(new DiagnosticMessage("SharedFixture Stopped..."));
    }

    public async Task ResetDatabasesAsync(CancellationToken cancellationToken = default)
    {
        //await MsSqlContainerFixture.ResetDbAsync(cancellationToken);
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
