namespace Vertical.Slice.Template.ApiClient.RickAndMorty.Dtos;

public record CharacterResponseClientDto(
    int Id,
    string Name,
    string Status,
    string Species,
    string Type,
    string Gender,
    OriginClientDto OriginClient,
    OriginClientDto Location,
    string Image,
    List<string> Episode,
    string Url,
    DateTime Created
);
