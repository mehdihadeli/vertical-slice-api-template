using System.Net.Http.Json;
using FluentAssertions;
using Vertical.Slice.Template.ApiClient.RickAndMorty.Dtos;

namespace Vertical.Slice.Template.ContractTests.RickAndMorty;

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
        var character = await response.Content.ReadFromJsonAsync<CharacterResponseClientDto>();

        // Assert
        character.Should().NotBeNull();
        character!.Id.Should().Be(characterId);
        character.Name.Should().Be(expectedName);
        character.Species.Should().Be(expectedSpecies);
        character.Gender.Should().Be(expectedGender);
    }
}
