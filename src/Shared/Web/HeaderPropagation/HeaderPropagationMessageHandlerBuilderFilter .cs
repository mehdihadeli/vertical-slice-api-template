using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Shared.Web.HeaderPropagation;

// `Microsoft.AspNetCore.HeaderPropagation` package is just client based and for applying it to all clients we should create a HeaderPropagationMessageHandlerBuilderFilter for it.
public class HeaderPropagationMessageHandlerBuilderFilter(
    IOptions<HeaderPropagationMessageHandlerOptions> options,
    HeaderPropagationValues header
) : IHttpMessageHandlerBuilderFilter
{
    private readonly HeaderPropagationMessageHandlerOptions _options = options.Value;

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            builder.AdditionalHandlers.Add(new HeaderPropagationMessageHandler(_options, header));
            next(builder);
        };
    }
}
