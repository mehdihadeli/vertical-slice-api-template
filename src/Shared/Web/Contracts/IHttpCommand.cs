using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Shared.Web.Contracts;

// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#positional-syntax-for-property-definition
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#nondestructive-mutation
// https://alexanderzeitler.com/articles/deconstructing-a-csharp-record-with-properties/
public interface IHttpCommand<TRequest>
    where TRequest : class
{
    TRequest Request { get; init; }
    HttpContext HttpContext { get; init; }
    IMediator Mediator { get; init; }
    IMapper Mapper { get; init; }
    CancellationToken CancellationToken { get; init; }
}
