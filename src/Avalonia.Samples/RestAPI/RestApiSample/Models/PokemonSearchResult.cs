using System.Collections.Generic;

namespace RestApiSample.Models;

public sealed record PokemonSearchResult(IReadOnlyList<PokemonDetails> Pokemons, int TotalCount);

public enum PokemonFilterKind
{
    Name,
    Type,
    Generation,
}
