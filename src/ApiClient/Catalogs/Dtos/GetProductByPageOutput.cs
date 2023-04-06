using Shared.Core.Wrappers;

namespace ApiClient.Catalogs.Dtos;

public record GetGetProductsByPageOutput(IPageList<ProductDto> Products);
