using RestApiSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RestApiSample.Services;

public class PokeApiClient : IPokeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PokeApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<PokemonType>> GetPokemonTypesAsync()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");

        PokemonTypeListResponse? response = await httpClient.GetFromJsonAsync<PokemonTypeListResponse>("type?limit=100");
        if (response is null)
        {
            return [];
        }

        return response.Results;
    }

    public async Task<List<PokemonGeneration>> GetPokemonGenerationsAsync()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");

        PokemonGenerationListResponse? response = await httpClient.GetFromJsonAsync<PokemonGenerationListResponse>("generation?limit=100");
        if (response is null)
        {
            return [];
        }

        return response.Results;
    }

    public async Task<PokemonSearchResult> SearchPokemonsAsync(
        string? name,
        string? typeName,
        string? generationName,
        int offset,
        int limit)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            PokemonDetails? pokemon = await GetPokemonByNameAsync(name);
            if (pokemon is null || !MatchesType(pokemon, typeName) || !await IsInGenerationAsync(pokemon.Name, generationName))
            {
                return new PokemonSearchResult([], 0);
            }

            return offset == 0
                ? new PokemonSearchResult([pokemon], 1)
                : new PokemonSearchResult([], 1);
        }

        IReadOnlyList<string>? pokemonNames = null;
        if (!string.IsNullOrWhiteSpace(typeName))
        {
            pokemonNames = await GetPokemonNamesByTypeAsync(typeName);
        }

        if (!string.IsNullOrWhiteSpace(generationName))
        {
            IReadOnlyList<string> generationPokemonNames = await GetPokemonNamesByGenerationAsync(generationName);
            pokemonNames = pokemonNames is null
                ? generationPokemonNames
                : pokemonNames.Intersect(generationPokemonNames, StringComparer.OrdinalIgnoreCase).ToList();
        }

        if (pokemonNames is null)
        {
            return await GetPokemonPageAsync(offset, limit);
        }

        int totalCount = pokemonNames.Count;
        IEnumerable<string> selectedNames = pokemonNames.Skip(offset).Take(limit);
        PokemonDetails?[] pokemons = await Task.WhenAll(selectedNames.Select(GetPokemonByNameAsync));

        return new PokemonSearchResult(
            pokemons
                .Where(pokemon => pokemon is not null)
                .Select(pokemon => pokemon!)
                .ToList(),
            totalCount);
    }

    private async Task<PokemonSearchResult> GetPokemonPageAsync(int offset, int limit)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        PokemonListResponse? response = await httpClient.GetFromJsonAsync<PokemonListResponse>(
            $"pokemon?offset={offset}&limit={limit}");

        if (response is null)
        {
            return new PokemonSearchResult([], 0);
        }

        IEnumerable<string> names = response.Results
            .Where(item => !string.IsNullOrWhiteSpace(item.Name))
            .Select(item => item.Name!);
        PokemonDetails?[] pokemons = await Task.WhenAll(names.Select(GetPokemonByNameAsync));
        return new PokemonSearchResult(
            pokemons
                .Where(pokemon => pokemon is not null)
                .Select(pokemon => pokemon!)
                .ToList(),
            response.Count);
    }

    private async Task<PokemonDetails?> GetPokemonByNameAsync(string name)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        PokemonDetails? pokemon = await httpClient.GetFromJsonAsync<PokemonDetails>(
            $"pokemon/{Uri.EscapeDataString(name)}");

        return pokemon;
    }

    private async Task<IReadOnlyList<string>> GetPokemonNamesByTypeAsync(string typeName)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        PokemonTypeDetails? response = await httpClient.GetFromJsonAsync<PokemonTypeDetails>(
            $"type/{Uri.EscapeDataString(typeName)}");

        IReadOnlyList<string> names = response?.Pokemon
            .Select(item => item.Pokemon.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToList() ?? [];

        return names;
    }

    private async Task<IReadOnlyList<string>> GetPokemonNamesByGenerationAsync(string generationName)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        PokemonGenerationDetails? response = await httpClient.GetFromJsonAsync<PokemonGenerationDetails>(
            $"generation/{Uri.EscapeDataString(generationName)}");

        IReadOnlyList<string> names = response?.PokemonSpecies
            .Where(item => !string.IsNullOrWhiteSpace(item.Name))
            .Select(item => item.Name!)
            .ToList() ?? [];

        return names;
    }

    private async Task<bool> IsInGenerationAsync(string pokemonName, string? generationName)
    {
        if (string.IsNullOrWhiteSpace(generationName))
        {
            return true;
        }

        IReadOnlyList<string> generationPokemonNames = await GetPokemonNamesByGenerationAsync(generationName);
        return generationPokemonNames.Contains(pokemonName, StringComparer.OrdinalIgnoreCase);
    }

    private bool MatchesType(PokemonDetails pokemon, string? typeName)
    {
        return string.IsNullOrWhiteSpace(typeName) || pokemon.Types.Any(type =>
            string.Equals(type.Type.Name, typeName, StringComparison.OrdinalIgnoreCase));
    }
}
