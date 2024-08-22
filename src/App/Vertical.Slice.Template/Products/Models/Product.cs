namespace Vertical.Slice.Template.Products.Models;

public class Product
{
    public required decimal Price { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
