using System.Collections.Generic;

namespace RestApiSample.Models;

public class PokemonTypeListResponse
{
    public int Count { get; init; }

    public string? Next { get; init; }

    public string? Previous { get; init; }

    public List<PokemonType> Results { get; init; } = [];
}

public class PokemonType
{
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}

public sealed class PokemonTypeDetails
{
    public List<PokemonTypePokemon> Pokemon { get; init; } = [];
}

public sealed class PokemonTypePokemon
{
    public PokemonListItem Pokemon { get; init; } = new(null, null);
}
