namespace Vertical.Slice.Template.Products.Dtos.v1;

public record ProductDto(Guid Id, Guid CategoryId, string Name, decimal Price, string? Description);
