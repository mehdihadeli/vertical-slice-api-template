using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Shared.Validation.Extensions;

namespace Shared.Web.ProblemDetail.HttpResults;

public class HttpProblemResultBase
    : IResult,
        IStatusCodeHttpResult,
        IContentTypeHttpResult,
        IValueHttpResult,
        IEndpointMetadataProvider,
        IValueHttpResult<ProblemDetails>
{
    private readonly ProblemHttpResult _problem;

    public HttpProblemResultBase(
        int statusCode,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null,
        IDictionary<string, object?>? extensions = null
    )
    {
        _problem = TypedResults.Problem(
            statusCode: statusCode,
            title: title,
            type: type,
            detail: detail,
            instance: instance,
            extensions: extensions
        );
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _problem.ExecuteAsync(httpContext);
    }

    public ProblemDetails ProblemDetails => _problem.ProblemDetails;
    public virtual int? StatusCode => _problem.StatusCode;
    public string ContentType => _problem.ContentType;
    object IValueHttpResult.Value => ProblemDetails;
    public ProblemDetails Value => ProblemDetails;

    /// <summary>
    /// This method will handle `auto generate` swagger metadata for the `problems` with implementing `IEndpointMetadataProvider`
    /// </summary>
    /// <param name="method"></param>
    /// <param name="builder"></param>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        method.NotNull();
        builder.NotNull();
        builder.Metadata.Add(new ResponseMetadata(StatusCodes.Status500InternalServerError, typeof(ProblemDetails)));
    }
}
