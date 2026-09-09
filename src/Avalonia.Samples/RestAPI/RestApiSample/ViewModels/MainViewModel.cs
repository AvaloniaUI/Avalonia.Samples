using CommunityToolkit.Mvvm.ComponentModel;
using RestApiSample.Models;
using RestApiSample.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestApiSample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly PokeApiClient _pokeApiClient;

    public MainViewModel(PokeApiClient pokeApiClient)
    {
        _pokeApiClient = pokeApiClient;
    }

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<PokemonListItem> Pokemons { get; set; } = [];

    public async Task LoadPokemonsAsync()
    {
        IsLoading = true;

        try
        {
            PokemonListResponse response = await _pokeApiClient.GetPokemonsAsync(0, 25);

            foreach (var item in response.Results)
            {
                Pokemons.Add(item);
            }
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = $"PokeAPI returned an invalid response.\n {ex.Message}";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to load Pokemons.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
