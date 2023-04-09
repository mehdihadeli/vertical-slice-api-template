namespace Vertical.Slice.Template.Products.ReadModel;

public class ProductReadModel
{
    public required decimal Price { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
