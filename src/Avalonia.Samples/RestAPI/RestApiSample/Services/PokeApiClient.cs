using RestApiSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RestApiSample.Services;

public class PokeApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PokeApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<PokemonDetails>> GetPokemonsAsync(int offset, int limit)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");

        PokemonListResponse? response = await httpClient.GetFromJsonAsync<PokemonListResponse>(
                $"pokemon?offset={offset}&limit={limit}");
        if (response is null)
        {
            return [];
        }

        List<PokemonDetails> results = [];

        foreach (PokemonListItem item in response.Results)
        {
            PokemonDetails? pokemon = await httpClient.GetFromJsonAsync<PokemonDetails>($"pokemon/{item.Name}");
            if (pokemon is null)
            {
                continue;
            }

            results.Add(pokemon);
        }

        return results;
    }

    public async Task<PokemonDetails?> GetPokemonByName(string name)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");

        return await httpClient.GetFromJsonAsync<PokemonDetails>(
            $"pokemon/{Uri.EscapeDataString(name)}");
    }

    public async Task<List<PokemonType>> GetPokemonTypes()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        
        PokemonTypeListResponse? response = await httpClient.GetFromJsonAsync<PokemonTypeListResponse>($"type");
        if (response is null)
        {
            return [];
        }

        return response.Results;
    }

    public async Task<List<PokemonDetails>> GetPokemonsByTypeAsync(string typeName, int limit)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("PokeApi");
        string escapedTypeName = Uri.EscapeDataString(typeName);

        PokemonTypeDetails? response = await httpClient.GetFromJsonAsync<PokemonTypeDetails>(
            $"type/{escapedTypeName}");
        if (response is null)
        {
            return [];
        }

        List<PokemonDetails> results = [];

        foreach (PokemonTypePokemon item in Enumerable.Take(response.Pokemon, limit))
        {
            if (string.IsNullOrWhiteSpace(item.Pokemon.Name))
            {
                continue;
            }

            PokemonDetails? pokemon = await GetPokemonByName(item.Pokemon.Name);
            if (pokemon is not null)
            {
                results.Add(pokemon);
            }
        }

        return results;
    }
}
