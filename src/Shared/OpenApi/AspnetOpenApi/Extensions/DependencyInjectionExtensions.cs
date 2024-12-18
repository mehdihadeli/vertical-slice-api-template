using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.OpenApi.AspnetOpenApi.Extensions;

// https://github.com/dotnet/aspnet-api-versioning/issues/1115

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder AddAspnetOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<AspnetOptions>().Bind(builder.Configuration.GetSection(nameof(AspnetOptions)));

        string[] versions = ["v1"];
        foreach (var documentName in versions)
        {
            builder.Services.AddOpenApi(
                documentName,
                options =>
                {
                    options.AddDocumentTransformer<OpenApiVersioningDocumentTransformer>();
                    options.AddDocumentTransformer<SecuritySchemeDocumentTransformer>();

                    options.AddOperationTransformer<OpenApiDefaultValuesOperationTransformer>();
                    options.AddOperationTransformer<CorrelationIdHeaderOperationTransformer>();

                    options.AddSchemaTransformer<EnumSchemaTransformer>();
                }
            );
        }

        return builder;
    }
}
