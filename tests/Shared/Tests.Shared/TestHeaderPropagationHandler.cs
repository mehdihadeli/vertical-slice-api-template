using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Vertical.Slice.Template.TestsShared;

public sealed class TestHeaderPropagationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var headers = httpContextAccessor.HttpContext?.Request.Headers;
        if (headers is not null && headers.TryGetValue("traceparent", out var traceParent))
            request.Headers.TryAddWithoutValidation("traceparent", traceParent.ToString());

        return base.SendAsync(request, cancellationToken);
    }
}
