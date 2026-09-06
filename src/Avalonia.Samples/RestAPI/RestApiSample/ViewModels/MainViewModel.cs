using CommunityToolkit.Mvvm.ComponentModel;
using RestApiSample.Models;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RestApiSample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<PokemonListItem> Pokemons { get; set; } = [];

    // TODO: Move API calls to service.
    public async Task LoadPokemonsAsync()
    {
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2/")
        };

        try
        {
            PokemonListResponse? response = await client.GetFromJsonAsync<PokemonListResponse>(
                "pokemon?offset=0&limit=50");

            if (response is null)
            {
                ErrorMessage = "PokeAPI returned an empty response.";
                return;
            }

            foreach (var item in response.Results)
            {
                Pokemons.Add(item);
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to load Pokemon.";
        }
    }
}
