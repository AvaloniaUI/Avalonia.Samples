using System.Collections.Generic;

namespace RestApiSample.Models;

public class PokemonListResponse
{
    public int Count { get; set; }

    public string? Next { get; set; }

    public string? Previous { get; set; }

    public List<PokemonListItem> Results { get; set; } = [];
}

public class PokemonListItem
{
    public string? Name { get; set; }

    public string? Url { get; set; }
}
