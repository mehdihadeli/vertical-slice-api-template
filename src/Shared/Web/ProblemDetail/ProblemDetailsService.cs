using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Web;

namespace Shared.Web.ProblemDetail;

// https://www.strathweb.com/2022/08/problem-details-responses-everywhere-with-asp-net-core-and-net-7/
public class ProblemDetailsService : IProblemDetailsService
{
    private readonly IEnumerable<IProblemDetailMapper>? _problemDetailMappers;
    private readonly IProblemDetailsWriter[] _writers;

    public ProblemDetailsService(
        IEnumerable<IProblemDetailsWriter> writers,
        IEnumerable<IProblemDetailMapper>? problemDetailMappers = null
    )
    {
        _writers = writers.ToArray();
        _problemDetailMappers = problemDetailMappers;
    }

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        ArgumentNullException.ThrowIfNull((object)context, nameof(context));
        ArgumentNullException.ThrowIfNull((object)context.ProblemDetails, "context.ProblemDetails");
        ArgumentNullException.ThrowIfNull((object)context.HttpContext, "context.HttpContext");

        // with help of `capture exception middleware` for capturing actual thrown exception
        var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature is null)
        {
            throw new Exception(
                "Please register `CaptureExceptionMiddleware` after `DeveloperExceptionPageMiddleware` in the middlewares list."
            );
        }

        if (_problemDetailMappers is { })
        {
            foreach (var problemDetailMapper in _problemDetailMappers)
            {
                var mappedStatusCode = problemDetailMapper.GetMappedStatusCodes(exceptionFeature.Error);
                if (mappedStatusCode > 0)
                {
                    PopulateNewProblemDetail(context.ProblemDetails, mappedStatusCode, exceptionFeature.Error);
                }
            }
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

        for (int index = 0; index < _writers.Length; ++index)
        {
            if (_writers[index].CanWrite(context))
            {
                problemDetailsWriter = _writers[index];
                break;
            }
        }

        return problemDetailsWriter?.WriteAsync(context) ?? ValueTask.CompletedTask;
    }

    private static void PopulateNewProblemDetail(
        ProblemDetails existingProblemDetails,
        int statusCode,
        Exception exception
    )
    {
        existingProblemDetails.Title = exception.GetType().FullName;
        existingProblemDetails.Detail = exception.Message;
        existingProblemDetails.Status = statusCode;
    }
}
