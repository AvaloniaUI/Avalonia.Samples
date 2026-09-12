using System.Collections.Generic;

namespace RestApiSample.Models;

public record PokemonListResponse(int Count, string? Next, string? Previous, List<PokemonListItem> Results);

public record PokemonListItem(string? Name, string? Url);
