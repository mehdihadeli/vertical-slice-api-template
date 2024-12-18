using Meziantou.Extensions.Logging.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Vertical.Slice.Template.Api;
using Environments = Shared.Web.Environments;

namespace Vertical.Slice.Template.TestsShared.Factory;

public class CustomWebApplicationFactory(Action<IWebHostBuilder>? webHostBuilder = null)
    : WebApplicationFactory<CatalogsApiMetadata>,
        IAsyncLifetime
{
    private ITestOutputHelper? _outputHelper;
    private readonly Dictionary<string, string?> _inMemoryConfigs = new();

    public Action<IServiceCollection>? TestConfigureServices { get; set; }
    public Action<IConfiguration>? ConfigurationAction { get; set; }
    public Action<WebHostBuilderContext, IConfigurationBuilder>? TestConfigureApp { get; set; }

    /// <summary>
    /// Use for tracking occured log events for testing purposes
    /// </summary>
    public InMemoryLoggerProvider InMemoryLogTrackerProvider { get; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Test);
        builder.UseContentRoot(".");

        // UseSerilog on WebHostBuilder is absolute so we should use IHostBuilder
        builder.UseSerilog(
            (ctx, loggerConfiguration) =>
            {
                // https://www.meziantou.net/how-to-test-the-logs-from-ilogger-in-dotnet.htm
                // We could also create a serilog sink for this in-memoryLoggerProvider for keep-tracking logs in the test and their states
                var loggerProviderCollections = new LoggerProviderCollection();
                loggerProviderCollections.AddProvider(InMemoryLogTrackerProvider);
                loggerConfiguration.WriteTo.Providers(loggerProviderCollections);

                //https://github.com/trbenning/serilog-sinks-xunit
                if (_outputHelper is { })
                {
                    loggerConfiguration.WriteTo.TestOutput(
                        _outputHelper,
                        LogEventLevel.Information,
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}"
                    );
                }
            }
        );

        // builder.ConfigureWebHost(wb =>
        // {
        //     wb.ConfigureTestServices(services => { });
        //
        //     wb.ConfigureAppConfiguration((hostingContext, configurationBuilder) => { });
        // });

        return base.CreateHost(builder);
    }

    public void SetOutputHelper(ITestOutputHelper value) => _outputHelper = value;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        webHostBuilder?.Invoke(builder);

        builder.ConfigureAppConfiguration(
            (hostingContext, configurationBuilder) =>
            {
                //// add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
                //// https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
                configurationBuilder.AddInMemoryCollection(_inMemoryConfigs);

                ConfigurationAction?.Invoke(hostingContext.Configuration);
                TestConfigureApp?.Invoke(hostingContext, configurationBuilder);
            }
        );

        builder.ConfigureTestServices(services =>
        {
            // https://andrewlock.net/converting-integration-tests-to-net-core-3/
            // Don't run IHostedServices when running as a test, we run them manually with `TestWorkersRunner` for more control
            services.RemoveAll<IHostedService>();

            TestConfigureServices?.Invoke(services);
        });

        base.ConfigureWebHost(builder);
    }

    public void AddOverrideInMemoryConfig(string key, string value)
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
        // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
        _inMemoryConfigs.Add(key, value);
    }

    public void AddOverrideInMemoryConfig(IDictionary<string, string> inMemConfigs)
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
        // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
        inMemConfigs.ToList().ForEach(x => _inMemoryConfigs.Add(x.Key, x.Value));
    }

    public void AddOverrideEnvKeyValue(string key, string value)
    {
        // overriding app configs with using environments
        Environment.SetEnvironmentVariable(key, value);
    }

    public void AddOverrideEnvKeyValues(IDictionary<string, string> keyValues)
    {
        foreach (var (key, value) in keyValues)
        {
            // overriding app configs with using environments
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
