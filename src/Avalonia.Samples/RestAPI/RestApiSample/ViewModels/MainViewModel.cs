using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestApiSample.Models;
using RestApiSample.Services;
using System;
using System.Collections.Generic;
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
    private string _pokemonName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<PokemonDetails> Pokemons { get; set; } = [];

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (PokemonName == string.Empty)
        {
            return;
        }

        IsLoading = true;

        try
        {
            PokemonDetails? response = await _pokeApiClient.GetPokemonByName(PokemonName);
            if (response is null)
            {
                ErrorMessage = "PokeAPI returned no data for this Pokemon.";
                IsLoading = false;
                return;
            }

            Pokemons.Clear();
            Pokemons.Add(response);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        PokemonName = string.Empty;
        await LoadPokemonsAsync();
    }

    public async Task LoadPokemonsAsync()
    {
        IsLoading = true;
        Pokemons.Clear();

        try
        {
            List<PokemonDetails> response = await _pokeApiClient.GetPokemonsAsync(0, 25);

            foreach (var item in response)
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
