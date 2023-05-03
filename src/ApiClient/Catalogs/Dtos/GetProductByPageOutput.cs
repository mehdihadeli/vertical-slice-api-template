using Shared.Abstractions.Core.Paging;

namespace ApiClient.Catalogs.Dtos;

public record GetGetProductsByPageOutput(IPageList<ProductDto> Products);
