using Shared.Abstractions.Core.Paging;

namespace Shared.Core.Paging;

// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records#characteristics-of-records
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
public record PageQuery<TResponse> : PageRequest, IPageQuery<TResponse>
    where TResponse : class;
