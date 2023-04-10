using Microsoft.AspNetCore.Http;

namespace Shared.Web.ProblemDetail.HttpResults;

public class InternalHttpProblemResult : HttpProblemResultBase
{
    public InternalHttpProblemResult(
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null,
        IDictionary<string, object?>? extensions = null
    )
        : base(StatusCodes.Status500InternalServerError, title, type, detail, instance, extensions) { }
}
