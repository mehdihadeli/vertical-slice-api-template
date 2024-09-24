using Mediator;
using Microsoft.AspNetCore.Http;
using Shared.Abstractions.Web;

namespace Shared.Web.Minimal;

public record HttpQuery(HttpContext HttpContext, IMediator Mediator, CancellationToken CancellationToken) : IHttpQuery;
