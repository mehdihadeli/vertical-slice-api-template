using Shared.Core.Wrappers;

namespace Catalogs.ApiClient.Catalogs.Dtos;

public record GetGetProductsByPageOutput(IPageList<ProductDto> Products);
