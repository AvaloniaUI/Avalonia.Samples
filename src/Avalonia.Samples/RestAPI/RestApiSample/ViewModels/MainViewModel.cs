using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestApiSample.Models;
using RestApiSample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    public async Task InitializeAsync()
    {
        List<PokemonType> pokemonTypes = await _pokeApiClient.GetPokemonTypes();

        foreach (PokemonType item in pokemonTypes)
        {
            AllPokemonType.Add(item);
        }

        await LoadPokemonsAsync();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    [NotifyCanExecuteChangedFor(nameof(ClearFiltersCommand))]
    private bool _isLoading;

    [ObservableProperty]
    private string _pokemonName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private PokemonType? _currentPokemonType;

    public ObservableCollection<PokemonDetails> Pokemons { get; set; } = [];

    public ObservableCollection<PokemonType> AllPokemonType { get; set; } = [];

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task SearchAsync()
    {
        string pokemonName = PokemonName.Trim();
        PokemonName = pokemonName;

        if (string.IsNullOrWhiteSpace(pokemonName) && CurrentPokemonType is null)
        {
            await LoadPokemonsAsync();
            return;
        }

        Pokemons.Clear();

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            if (!string.IsNullOrWhiteSpace(pokemonName))
            {
                PokemonDetails? response = await _pokeApiClient.GetPokemonByName(pokemonName);
                if (response is null || !MatchesSelectedType(response))
                {
                    ErrorMessage = "PokeAPI returned no data for the selected filters.";
                    return;
                }

                Pokemons.Add(response);
                return;
            }

            List<PokemonDetails> responseByType = await _pokeApiClient.GetPokemonsByTypeAsync(
                CurrentPokemonType!.Name,
                25);

            foreach (PokemonDetails pokemon in responseByType)
            {
                Pokemons.Add(pokemon);
            }

            if (Pokemons.Count == 0)
            {
                ErrorMessage = "PokeAPI returned no data for the selected filters.";
            }
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
        CurrentPokemonType = null;
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
        catch (JsonException)
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

    private bool MatchesSelectedType(PokemonDetails pokemon)
    {
        return CurrentPokemonType is null || pokemon.Types.Any(type =>
            string.Equals(type.Type.Name, CurrentPokemonType.Name, StringComparison.OrdinalIgnoreCase));
    }
}
