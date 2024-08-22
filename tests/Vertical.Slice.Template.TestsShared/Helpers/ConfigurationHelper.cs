using Microsoft.Extensions.Configuration;
using Shared.Core.Extensions;

namespace Vertical.Slice.Template.TestsShared.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfigurationRoot _configurationRoot;

    static ConfigurationHelper()
    {
        _configurationRoot = BuildConfiguration();
    }

    public static TOptions BindOptions<TOptions>()
        where TOptions : new()
    {
        return _configurationRoot.BindOptions<TOptions>(typeof(TOptions).Name);
    }

    //https://stackoverflow.com/questions/39791634/read-appsettings-json-values-in-net-core-test-project
    //https://www.thecodebuzz.com/read-appsettings-json-in-net-core-test-project-xunit-mstest/
    //https://weblog.west-wind.com/posts/2018/Feb/18/Accessing-Configuration-in-NET-Core-Test-Projects
    //https://bartwullems.blogspot.com/2019/03/net-coreunit-tests-configuration.html
    private static IConfigurationRoot BuildConfiguration()
    {
        var rootPath = Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.test.json", optional: true)
            .Build();

        return builder;
    }
}
