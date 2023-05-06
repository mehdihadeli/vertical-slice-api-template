using System.Net.Http.Json;
using FluentAssertions;

namespace Vertical.Slice.Template.ContractTests.RickAndMortyApi;

public class RickAndMortyApiTests
{
    private readonly HttpClient _httpClient;

    public RickAndMortyApiTests()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://rickandmortyapi.com"), };
    }

    [Fact]
    public async Task contracts_should_pass_for_get_character_by_id()
    {
        // Arrange
        int characterId = 1;
        string expectedName = "Rick Sanchez";
        string expectedSpecies = "Human";
        string expectedGender = "Male";

        // Act
        var response = await _httpClient.GetAsync("/api/character/1");
        response.EnsureSuccessStatusCode();
        var character = await response.Content.ReadFromJsonAsync<CharacterResponseDto>();

        // Assert
        character.Should().NotBeNull();
        character!.Id.Should().Be(characterId);
        character.Name.Should().Be(expectedName);
        character.Species.Should().Be(expectedSpecies);
        character.Gender.Should().Be(expectedGender);
    }
}

public record CharacterResponseDto(
    int Id,
    string Name,
    string Status,
    string Species,
    string Type,
    string Gender,
    OriginDto Origin,
    OriginDto Location,
    string Image,
    List<string> Episode,
    string Url,
    DateTime Created
);

public record OriginDto(string Name, string Url);

public record LocationDto(string Name, string Url);
