using RestApiSample.Models;
using System;
using System.Collections.Generic;
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
     
        return await httpClient.GetFromJsonAsync<PokemonDetails>($"pokemon/{name}");
    }
}
