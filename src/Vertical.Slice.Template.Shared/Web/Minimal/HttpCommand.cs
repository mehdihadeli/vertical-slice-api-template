using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vertical.Slice.Template.Shared.Abstractions.Web;

namespace Vertical.Slice.Template.Shared.Web.Minimal;

public record HttpCommand<TRequest>(
    TRequest Request,
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand<TRequest>;
