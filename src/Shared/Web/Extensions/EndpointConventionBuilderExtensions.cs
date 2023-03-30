using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Shared.Web.Extensions;

public static class EndpointConventionBuilderExtensions
{
    public static RouteHandlerBuilder Produces(this RouteHandlerBuilder builder, int statusCode, string description)
    {
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });
        builder.Produces(statusCode);

        // // with suing https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcoreannotations
        // builder.WithMetadata(new SwaggerResponseAttribute(
        //     statusCode,
        //     description));

        return builder;
    }

    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        int statusCode,
        string description
    )
    {
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });
        builder.Produces<TResponse>(statusCode);

        // // with suing https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcoreannotations
        // builder.WithMetadata(new SwaggerResponseAttribute(
        //     statusCode,
        //     description,
        //     typeof(TResponse)));

        return builder;
    }
}
