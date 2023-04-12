using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Web.Contracts;

namespace Shared.Web.Minimal;

public record HttpQuery(
    HttpContext HttpContext,
    IMediator Mediator,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpQuery;
