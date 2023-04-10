using Microsoft.AspNetCore.Http;

namespace Shared.Web.ProblemDetail.HttpResults;

public class UnAuthorizedHttpProblemResult : HttpProblemResultBase
{
    public UnAuthorizedHttpProblemResult(
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null,
        IDictionary<string, object?>? extensions = null
    )
        : base(StatusCodes.Status401Unauthorized, title, type, detail, instance, extensions) { }
}
