using Shared.Core.Paging;

namespace Vertical.Slice.Template.Shared.Clients.Dtos;

public record GetProductByPageClientResponseDto(PageList<ProductClientDto> Products);
