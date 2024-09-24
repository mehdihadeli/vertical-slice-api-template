using Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Abstractions.Web;

namespace Shared.Web.Minimal;

public record HttpCommand<TRequest>(
    TRequest Request,
    HttpContext HttpContext,
    IMediator Mediator,
    CancellationToken CancellationToken
) : IHttpCommand<TRequest>;
