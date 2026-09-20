using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestApiSample.Models;

public sealed class PokemonDetails
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int Height { get; init; }

    public required int Weight { get; init; }

    [JsonPropertyName("base_experience")]
    public required int BaseExperience { get; init; }

    public List<PokemonTypeSlot> Types { get; init; } = [];
}

public sealed class PokemonTypeSlot
{
    public PokemonType Type { get; init; } = new();
}
