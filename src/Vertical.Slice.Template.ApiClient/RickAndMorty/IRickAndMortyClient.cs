using Vertical.Slice.Template.ApiClient.RickAndMorty.Model;

namespace Vertical.Slice.Template.ApiClient.RickAndMorty;

// Ref: https://learn.microsoft.com/en-us/azure/architecture/patterns/anti-corruption-layer
// Ref: https://deviq.com/domain-driven-design/anti-corruption-layer

/// <summary>
/// RickAndMortyClient acts as a anti-corruption-layer for our system.
/// An Anti-Corruption Layer (ACL) is a set of patterns placed between the domain model and other bounded contexts or third party dependencies. The intent of this layer is to prevent the intrusion of foreign concepts and models into the domain model.
/// </summary>
public interface IRickAndMortyClient
{
    Task<Character> GetCharacterById(long id, CancellationToken cancellationToken = default);
}
