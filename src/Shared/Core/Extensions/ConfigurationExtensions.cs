﻿using Microsoft.Extensions.Configuration;

namespace Shared.Core.Extensions;

/// <summary>
/// Static helper class for <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Attempts to bind the <typeparamref name="TOptions"/> instance to configuration section values.
    /// </summary>
    /// <typeparam name="TOptions">The given bind model.</typeparam>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <param name="section">The configuration section.</param>
    /// <param name="configurator"></param>
    /// <returns>The new instance of <typeparamref name="TOptions"/>.</returns>
    public static TOptions BindOptions<TOptions>(
        this IConfiguration configuration,
        string section,
        Action<TOptions>? configurator = null
    )
        where TOptions : new()
    {
        // note: with using Get<>() if there is no configuration in appsettings it just returns default value (null) for the configuration type
        // but if we use Bind() we can pass a instantiated type with its default value (for example in its property initialization) to bind method for binding configurations from appsettings
        // https://www.twilio.com/blog/provide-default-configuration-to-dotnet-applications
        var options = new TOptions();

        var optionsSection = configuration.GetSection(section);
        optionsSection.Bind(options);

        configurator?.Invoke(options);

        return options;
    }

    /// <summary>
    /// Attempts to bind the <typeparamref name="TOptions"/> instance to configuration section values.
    /// </summary>
    /// <typeparam name="TOptions">The given bind model.</typeparam>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <param name="configurator"></param>
    /// <returns>The new instance of <typeparamref name="TOptions"/>.</returns>
    public static TOptions BindOptions<TOptions>(
        this IConfiguration configuration,
        Action<TOptions>? configurator = null
    )
        where TOptions : new()
    {
        return BindOptions(configuration, typeof(TOptions).Name, configurator);
    }
}
