using RestApiSample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiSample.Services;

public interface IPokeApiClient
{
    Task<List<PokemonType>> GetPokemonTypesAsync();

    Task<List<PokemonGeneration>> GetPokemonGenerationsAsync();

    Task<List<PokemonDetails>> SearchPokemonsAsync(
        string? name,
        string? typeName,
        string? generationName,
        int limit);
}
