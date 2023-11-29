using Meziantou.Extensions.Logging.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Vertical.Slice.Template.Api;

namespace Vertical.Slice.Template.TestsShared.Factory;

public class CustomWebApplicationFactory : WebApplicationFactory<CatalogsApiMetadata>, IAsyncLifetime
{
    private ITestOutputHelper? _outputHelper;
    private readonly Action<IWebHostBuilder>? _webHostBuilder;

    public Action<IServiceCollection>? TestConfigureServices { get; set; }

    public Action<WebHostBuilderContext, IConfigurationBuilder>? TestConfigureApp { get; set; }

    /// <summary>
    /// Use for tracking occured log events for testing purposes
    /// </summary>
    public InMemoryLoggerProvider InMemoryLogTrackerProvider { get; } = new();

    public CustomWebApplicationFactory(Action<IWebHostBuilder>? webHostBuilder = null)
    {
        _webHostBuilder = webHostBuilder;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("test");

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
        _webHostBuilder?.Invoke(builder);

        builder.ConfigureAppConfiguration(
            (hostingContext, configurationBuilder) =>
            {
                TestConfigureApp?.Invoke(hostingContext, configurationBuilder);
            }
        );

        builder.ConfigureTestServices(services =>
        {
            TestConfigureServices?.Invoke(services);
        });

        base.ConfigureWebHost(builder);
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
