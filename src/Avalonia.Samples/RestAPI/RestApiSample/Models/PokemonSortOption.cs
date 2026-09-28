using System;

namespace RestApiSample.Models;

public enum PokemonSortField
{
    Number,
    Name,
}

public enum SortDirection
{
    Ascending,
    Descending,
}

public sealed record PokemonSortOption<T>(T Value, string DisplayName) where T : struct, Enum;
