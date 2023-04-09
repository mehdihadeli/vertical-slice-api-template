namespace Catalogs.Products.Dtos;

public record ProductDto(Guid Id, Guid CategoryId, string Name, decimal Price, string? Description);
