using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestApiSample.Models;

public sealed class PokemonGeneration
{
    public string Name { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;
}

public sealed class PokemonGenerationListResponse
{
    public List<PokemonGeneration> Results { get; init; } = [];
}

public sealed class PokemonGenerationDetails
{
    [JsonPropertyName("pokemon_species")]
    public List<PokemonListItem> PokemonSpecies { get; init; } = [];
}
