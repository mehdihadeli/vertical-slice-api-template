using Meziantou.Extensions.Logging.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Environments = Shared.Web.Environments;

namespace Vertical.Slice.Template.TestsShared.Factory;

public class CustomWebApplicationFactory<TRootMetadata>(Action<IWebHostBuilder>? webHostBuilder = null)
    : WebApplicationFactory<TRootMetadata>
    where TRootMetadata : class
{
    private ITestOutputHelper? _outputHelper;
    private readonly Dictionary<string, string?> _inMemoryConfigs = new();
    private Action<IServiceCollection>? _testConfigureServices;
    private Action<IConfiguration>? _testConfiguration;
    private Action<WebHostBuilderContext, IConfigurationBuilder>? _testConfigureAppConfiguration;
    private readonly List<Type> _testHostedServicesTypes = new();
    private readonly Dictionary<string, string?> _originalEnvironmentValues = new();

    /// <summary>
    /// Use for tracking occured log events for testing purposes
    /// </summary>
    public InMemoryLoggerProvider InMemoryLogTrackerProvider { get; } = new();

    public CustomWebApplicationFactory<TRootMetadata> WithTestConfigureServices(Action<IServiceCollection> services)
    {
        _testConfigureServices += services;
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> WithTestConfiguration(Action<IConfiguration> configurations)
    {
        _testConfiguration += configurations;
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> WithTestConfigureAppConfiguration(
        Action<WebHostBuilderContext, IConfigurationBuilder> appConfigurations
    )
    {
        _testConfigureAppConfiguration += appConfigurations;
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> AddTestHostedService<THostedService>()
        where THostedService : class, IHostedService
    {
        _testHostedServicesTypes.Add(typeof(THostedService));
        return this;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Test);
        builder.UseContentRoot(".");

        builder.ConfigureLogging(logging => logging.AddProvider(InMemoryLogTrackerProvider));

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

                _testConfiguration?.Invoke(hostingContext.Configuration);
                _testConfigureAppConfiguration?.Invoke(hostingContext, configurationBuilder);
            }
        );

        builder.ConfigureTestServices(services =>
        {
            // https://andrewlock.net/converting-integration-tests-to-net-core-3/
            // - we delete default hosted services like `GenericWebHostService` which is for hosting app on the application port and host, and we just add required test hosted services
            // - `WebApplicationFactory` create a new `GenericWebHostService` test server and add it as a `IHostedService` to the list of hosted services
            // - we run them manually some of them with `TestWorkersRunner` for more control
            foreach (var descriptor in services
                .Where(descriptor => descriptor.ServiceType == typeof(IHostedService)
                    && descriptor.ImplementationType?.Name == "GenericWebHostService")
                .ToList())
            {
                services.Remove(descriptor);
            }

            // add test hosted services
            foreach (var hostedServiceType in _testHostedServicesTypes)
            {
                services.AddSingleton(typeof(IHostedService), hostedServiceType);
            }

            _testConfigureServices?.Invoke(services);
        });

        base.ConfigureWebHost(builder);
    }

    public CustomWebApplicationFactory<TRootMetadata> AddOverrideInMemoryConfig(string key, string value)
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
        // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
        _inMemoryConfigs.Add(key, value);
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> AddOverrideInMemoryConfig(
        IDictionary<string, string> inMemConfigs
    )
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
        // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
        inMemConfigs.ToList().ForEach(x => _inMemoryConfigs.Add(x.Key, x.Value));
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> AddOverrideEnvKeyValue(string key, string value)
    {
        // overriding app configs with using environments
        RememberEnvironmentValue(key);
        Environment.SetEnvironmentVariable(key, value);
        return this;
    }

    public CustomWebApplicationFactory<TRootMetadata> AddOverrideEnvKeyValues(
        IDictionary<string, string> keyValues
    )
    {
        foreach (var (key, value) in keyValues)
        {
            // overriding app configs with using environments
            RememberEnvironmentValue(key);
            Environment.SetEnvironmentVariable(key, value);
        }

        return this;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var (key, value) in _originalEnvironmentValues)
                Environment.SetEnvironmentVariable(key, value);
        }

        base.Dispose(disposing);
    }

    private void RememberEnvironmentValue(string key)
    {
        if (!_originalEnvironmentValues.ContainsKey(key))
            _originalEnvironmentValues[key] = Environment.GetEnvironmentVariable(key);
    }
}
