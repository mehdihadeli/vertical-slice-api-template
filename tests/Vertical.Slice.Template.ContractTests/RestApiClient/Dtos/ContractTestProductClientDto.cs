namespace Vertical.Slice.Template.ContractTests.RestApiClient.Dtos;

public record ContractTestProductClientDto(Guid Id, Guid CategoryId, string Name, decimal Price, string? Description);
