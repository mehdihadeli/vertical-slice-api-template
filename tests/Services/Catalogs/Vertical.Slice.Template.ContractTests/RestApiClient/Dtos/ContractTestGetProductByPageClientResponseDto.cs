using Shared.Core.Paging;

namespace Vertical.Slice.Template.ContractTests.RestApiClient.Dtos;

internal record ContractTestGetProductByPageClientResponseDto(PageList<ContractTestProductClientDto> Products);
