using MediatR;

namespace Shared.Core.Types;

// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records#characteristics-of-records
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
public record PageQuery<TResponse> : PageRequest, IRequest<TResponse>
    where TResponse : class;
