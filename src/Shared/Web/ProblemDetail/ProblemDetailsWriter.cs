using Microsoft.AspNetCore.Http;

namespace Shared.Web.ProblemDetail;

// https://www.strathweb.com/2022/08/problem-details-responses-everywhere-with-asp-net-core-and-net-7/#toc_3
public class ProblemDetailsWriter : IProblemDetailsWriter
{
    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        return new ValueTask();
    }

    public bool CanWrite(ProblemDetailsContext context)
    {
        return true;
    }
}
