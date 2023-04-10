using Microsoft.AspNetCore.Http;

namespace Shared.Web.ProblemDetail.HttpResults;

public class NotFoundHttpProblemResult : HttpProblemResultBase
{
    public NotFoundHttpProblemResult(
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null,
        IDictionary<string, object?>? extensions = null
    )
        : base(StatusCodes.Status404NotFound, title, type, detail, instance, extensions) { }
}
