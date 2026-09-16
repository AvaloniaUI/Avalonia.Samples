using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestApiSample.Models;
using RestApiSample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    [NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))]
    private bool _isLoading;

    [ObservableProperty]
    private string _pokemonName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<PokemonDetails> Pokemons { get; } = [];

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(PokemonName))
        {
            return;
        }

        Pokemons.Clear();

        string pokemonName = PokemonName.Trim();
        PokemonName = pokemonName;
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            PokemonDetails? response = await _pokeApiClient.GetPokemonByName(pokemonName);
            if (response is null)
            {
                ErrorMessage = "PokeAPI returned no data for this Pokemon.";
                return;
            }

            Pokemons.Add(response);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "PokeAPI returned no data for this Pokemon.";
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to load Pokemon.";
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = "PokeAPI returned an invalid response.";
        }
        catch (JsonException)
        {
            ErrorMessage = "PokeAPI returned an invalid response.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task ClearFilters()
    {
        PokemonName = string.Empty;
        await LoadPokemonsAsync();
    }

    public async Task LoadPokemonsAsync()
    {
        if (IsLoading)
        {
            return;
        }

        Pokemons.Clear();

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            List<PokemonDetails> response = await _pokeApiClient.GetPokemonsAsync(0, 25);

            foreach (PokemonDetails item in response)
            {
                Pokemons.Add(item);
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to load Pokemons.";
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = "PokeAPI returned an invalid response.";
        }
        catch (System.Text.Json.JsonException)
        {
            ErrorMessage = "PokeAPI returned an invalid response.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanExecuteCommands()
    {
        return !IsLoading;
    }
}
