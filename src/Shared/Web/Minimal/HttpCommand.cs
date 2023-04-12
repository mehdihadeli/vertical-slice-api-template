using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Web.Contracts;

namespace Shared.Web.Minimal;

public record HttpCommand<TRequest>(
    TRequest Request,
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<TRequest>
    where TRequest : class;
