using MediatR;

namespace Shared.Core.Wrappers;

public record PageQuery<TResponse>(int PageSize, int PageNumber, string? Filters = null, string? SortOrder = null)
    : PageRequest(PageSize, PageNumber, Filters, SortOrder),
        IRequest<TResponse>
    where TResponse : class;
