namespace Vertical.Slice.Template.Products.Dtos;

public record ProductDto(Guid Id, Guid CategoryId, string Name, decimal Price, string? Description);
