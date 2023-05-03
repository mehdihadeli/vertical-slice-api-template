using Microsoft.AspNetCore.Http;
using Shared.Abstractions.Web;
using Shared.Core.Exceptions;
using Shared.Validation;
using Shared.Validation.Extensions;

namespace Shared.Web.ProblemDetail;

internal sealed class DefaultProblemDetailMapper : IProblemDetailMapper
{
    public int GetMappedStatusCodes(Exception exception)
    {
        return exception switch
        {
            ConflictException conflictException => conflictException.StatusCode,
            ValidationException validationException => validationException.StatusCode,
            ArgumentException _ => StatusCodes.Status400BadRequest,
            BadRequestException badRequestException => badRequestException.StatusCode,
            NotFoundException notFoundException => notFoundException.StatusCode,
            HttpResponseException httpResponseException => httpResponseException.StatusCode,
            HttpRequestException httpRequestException => (int)httpRequestException.StatusCode,
            AppException appException => appException.StatusCode,
            _ => 0
        };
    }
}
