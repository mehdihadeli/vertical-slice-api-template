namespace ApiClient.Catalogs.Dtos;

public record CreateProductClientDto(string Name, Guid CategoryId, decimal Price, string? Description);
