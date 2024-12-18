using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.OpenApi.Swashbuckle;

// https://andrewlock.net/simplifying-dependency-injection-for-iconfigureoptions-with-the-configureoptions-helper/
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureSwaggerGenVersioningOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
    private readonly SwaggerOptions _swaggerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerGenVersioningOptions"/> class.
    /// </summary>
    /// <param name="apiVersionDescriptionProvider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="options"></param>
    public ConfigureSwaggerGenVersioningOptions(
        IApiVersionDescriptionProvider apiVersionDescriptionProvider,
        IOptions<SwaggerOptions> options
    )
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        _swaggerOptions = options.Value;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder(_swaggerOptions.Description);
        var info = new OpenApiInfo
        {
            Title = _swaggerOptions.Title,
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = _swaggerOptions.ContactUserName,
                Email = _swaggerOptions.ContactEmail,
            },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") },
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                text.Append(" The API will be sunset on ").Append(when.Date.ToShortDateString()).Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                foreach (var link in policy.Links)
                {
                    if (link.Type == "text/html")
                    {
                        if (!rendered)
                        {
                            text.Append("<h4>Links</h4><ul>");
                            rendered = true;
                        }

                        text.Append("<li><a href=\"");
                        text.Append(link.LinkTarget.OriginalString);
                        text.Append("\">");
                        text.Append(
                            StringSegment.IsNullOrEmpty(link.Title)
                                ? link.LinkTarget.OriginalString
                                : link.Title.ToString()
                        );
                        text.Append("</a></li>");
                    }
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        text.Append("<h4>Additional Information</h4>");
        info.Description = text.ToString();

        return info;
    }
}
