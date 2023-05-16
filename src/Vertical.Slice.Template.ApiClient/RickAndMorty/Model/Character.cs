namespace Vertical.Slice.Template.ApiClient.RickAndMorty.Model;

public record Character(
    int Id,
    string Name,
    string Status,
    string Species,
    string Type,
    string Gender,
    Origin OriginClient,
    Origin Location,
    string Image,
    List<string> Episode,
    string Url,
    DateTime Created
);
