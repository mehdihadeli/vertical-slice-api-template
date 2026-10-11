namespace Vertical.Slice.Template.ContractTests.RestApiClient.Dtos;

public record ContractTestCreateProductClientRequestDto(
    string Name,
    Guid CategoryId,
    decimal Price,
    string? Description
);
