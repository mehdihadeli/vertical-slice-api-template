using Shared.Core.Types;

namespace ApiClient.Catalogs.Dtos;

public record GetGetProductsByPageOutput(IPageList<ProductDto> Products);
