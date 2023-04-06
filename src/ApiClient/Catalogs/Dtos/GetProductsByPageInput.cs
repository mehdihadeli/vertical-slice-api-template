using Shared.Core.Wrappers;

namespace ApiClient.Catalogs.Dtos;

public record GetGetProductsByPageInput(int PageSize, int PageNumber, string? Filters = null, string? SortOrder = null)
    : PageRequest(PageSize, PageNumber, Filters, SortOrder);
