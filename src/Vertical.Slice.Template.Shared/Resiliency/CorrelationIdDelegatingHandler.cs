using CorrelationId;
using CorrelationId.Abstractions;
using Microsoft.Extensions.Options;

namespace Vertical.Slice.Template.Shared.Resiliency;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly ICorrelationContextAccessor? _correlationContextAccessor;
    private readonly IOptions<CorrelationIdOptions>? _options;

    public CorrelationIdDelegatingHandler(
        ICorrelationContextAccessor? correlationContextAccessor = null,
        IOptions<CorrelationIdOptions>? options = null
    )
    {
        _correlationContextAccessor = correlationContextAccessor;
        _options = options;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (
            _correlationContextAccessor is not null
            && _options is not null
            && request.Headers.Contains(_options.Value.RequestHeader) == false
        )
        {
            request.Headers.Add(
                _options.Value.RequestHeader,
                _correlationContextAccessor.CorrelationContext?.CorrelationId
            );
        }

        // Else the header has already been added due to a retry.
        return base.SendAsync(request, cancellationToken);
    }
}
