namespace ApiClient.Catalogs.Dtos;

public record CreateProductInput(string Name, Guid CategoryId, decimal Price, string? Description);
