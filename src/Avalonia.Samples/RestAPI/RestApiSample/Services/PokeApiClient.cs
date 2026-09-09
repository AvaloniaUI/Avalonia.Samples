using RestApiSample.Models;
using System;
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

    public async Task<PokemonListResponse> GetPokemonsAsync(int offset, int limit)
    {
        var httpClient = _httpClientFactory.CreateClient("PokeApi");

        return await httpClient.GetFromJsonAsync<PokemonListResponse>(
                $"pokemon?offset={offset}&limit={limit}")
            ?? throw new InvalidOperationException("PokeAPI returned an empty response.");
    }
}
