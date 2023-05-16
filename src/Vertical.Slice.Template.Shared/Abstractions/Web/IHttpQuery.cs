using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Vertical.Slice.Template.Shared.Abstractions.Web;

// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#positional-syntax-for-property-definition
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#nondestructive-mutation
// https://alexanderzeitler.com/articles/deconstructing-a-csharp-record-with-properties/
public interface IHttpQuery
{
    HttpContext HttpContext { get; init; }
    IMediator Mediator { get; init; }
    IMapper Mapper { get; init; }
    CancellationToken CancellationToken { get; init; }
}
