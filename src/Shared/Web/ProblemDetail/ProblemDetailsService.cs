using Humanizer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Web;

namespace Shared.Web.ProblemDetail;

// https://www.strathweb.com/2022/08/problem-details-responses-everywhere-with-asp-net-core-and-net-7/
public class ProblemDetailsService(
    IEnumerable<IProblemDetailsWriter> writers,
    IEnumerable<IProblemDetailMapper> problemDetailMappers
) : IProblemDetailsService
{
    private readonly IProblemDetailsWriter[] _writers = writers.ToArray();

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(context.ProblemDetails);
        ArgumentNullException.ThrowIfNull(context.HttpContext);

        // with help of `capture exception middleware` for capturing actual thrown exception, in .net 8 preview 5 it will create automatically
        IExceptionHandlerFeature? exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();

        // if we throw an exception, we should create appropriate ProblemDetail based on the exception, else we just return default ProblemDetail with status 500 or a custom ProblemDetail which is returned from the endpoint
        if (exceptionFeature is not null)
        {
            CreateProblemDetailFromException(context, exceptionFeature);
        }

        if (
            context.HttpContext.Response.HasStarted
            || context.HttpContext.Response.StatusCode < 400
            || _writers.Length == 0
        )
            return ValueTask.CompletedTask;

        IProblemDetailsWriter problemDetailsWriter = null!;
        if (_writers.Length == 1)
        {
            IProblemDetailsWriter writer = _writers[0];
            return !writer.CanWrite(context) ? ValueTask.CompletedTask : writer.WriteAsync(context);
        }

        foreach (var writer in _writers)
        {
            if (writer.CanWrite(context))
            {
                problemDetailsWriter = writer;
                break;
            }
        }

        return problemDetailsWriter?.WriteAsync(context) ?? ValueTask.CompletedTask;
    }

    private void CreateProblemDetailFromException(
        ProblemDetailsContext context,
        IExceptionHandlerFeature exceptionFeature
    )
    {
        if (problemDetailMappers.Any())
        {
            foreach (var problemDetailMapper in problemDetailMappers)
            {
                MapProblemDetail(context, exceptionFeature, problemDetailMapper);
            }
        }
        else
        {
            var defaultMapper = new DefaultProblemDetailMapper();
            MapProblemDetail(context, exceptionFeature, defaultMapper);
        }
    }

    private static void MapProblemDetail(
        ProblemDetailsContext context,
        IExceptionHandlerFeature exceptionFeature,
        IProblemDetailMapper problemDetailMapper
    )
    {
        var mappedStatusCode = problemDetailMapper.GetMappedStatusCodes(exceptionFeature.Error);
        if (mappedStatusCode > 0)
        {
            PopulateNewProblemDetail(
                context.ProblemDetails,
                context.HttpContext,
                mappedStatusCode,
                exceptionFeature.Error
            );
            context.HttpContext.Response.StatusCode = mappedStatusCode;
        }
    }

    private static void PopulateNewProblemDetail(
        ProblemDetails existingProblemDetails,
        HttpContext httpContext,
        int statusCode,
        Exception exception
    )
    {
        // We should override ToString method in the exception for showing correct title.
        existingProblemDetails.Title = exception.GetType().Name.Humanize(LetterCasing.Title);
        existingProblemDetails.Detail = exception.Message;
        existingProblemDetails.Status = statusCode;
        existingProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
    }
}
