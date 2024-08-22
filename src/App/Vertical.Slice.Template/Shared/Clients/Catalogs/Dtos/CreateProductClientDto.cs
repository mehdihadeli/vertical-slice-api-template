namespace Vertical.Slice.Template.Shared.Clients.Catalogs.Dtos;

public record CreateProductClientDto(string Name, Guid CategoryId, decimal Price, string? Description);
