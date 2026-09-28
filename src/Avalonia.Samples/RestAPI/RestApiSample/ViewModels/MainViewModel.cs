using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestApiSample.Models;
using RestApiSample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApiSample.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const int ResultLimit = 25;
    private readonly IPokeApiClient _pokeApiClient;

    public MainViewModel(IPokeApiClient pokeApiClient)
    {
        _pokeApiClient = pokeApiClient;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            List<PokemonType> pokemonTypes = await _pokeApiClient.GetPokemonTypesAsync();
            List<PokemonGeneration> pokemonGenerations = await _pokeApiClient.GetPokemonGenerationsAsync();

            foreach (PokemonType item in pokemonTypes)
            {
                AllPokemonTypes.Add(item);
            }

            foreach (PokemonGeneration item in pokemonGenerations)
            {
                AllPokemonGenerations.Add(item);
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to load filters from PokeAPI.";
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

    [ObservableProperty]
    private PokemonGeneration? _currentPokemonGeneration;

    [ObservableProperty]
    private PokemonSortOption<PokemonSortField> _currentSortField = new(PokemonSortField.Number, "Number");

    [ObservableProperty]
    private PokemonSortOption<SortDirection> _currentSortDirection = new(SortDirection.Ascending, "Ascending");

    public ObservableCollection<PokemonDetails> Pokemons { get; } = [];

    public ObservableCollection<PokemonType> AllPokemonTypes { get; } = [];

    public ObservableCollection<PokemonGeneration> AllPokemonGenerations { get; } = [];

    public IReadOnlyList<PokemonSortOption<PokemonSortField>> SortFields { get; } =
    [
        new(PokemonSortField.Number, "Number"),
        new(PokemonSortField.Name, "Name"),
    ];

    public IReadOnlyList<PokemonSortOption<SortDirection>> SortDirections { get; } =
    [
        new(SortDirection.Ascending, "Ascending"),
        new(SortDirection.Descending, "Descending"),
    ];

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task SearchAsync()
    {
        string pokemonName = PokemonName.Trim();
        PokemonName = pokemonName;

        await LoadPokemonsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task ClearFilters()
    {
        PokemonName = string.Empty;
        CurrentPokemonType = null;
        CurrentPokemonGeneration = null;
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
            List<PokemonDetails> response = await _pokeApiClient.SearchPokemonsAsync(
                PokemonName,
                CurrentPokemonType?.Name,
                CurrentPokemonGeneration?.Name,
                ResultLimit);

            ReplacePokemons(response);

            if (Pokemons.Count == 0)
            {
                ErrorMessage = "PokeAPI returned no data for the selected filters.";
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

    private void ReplacePokemons(IEnumerable<PokemonDetails> pokemons)
    {
        Pokemons.Clear();

        foreach (PokemonDetails pokemon in SortPokemons(pokemons))
        {
            Pokemons.Add(pokemon);
        }
    }

    private void ApplySort()
    {
        if (Pokemons.Count > 1)
        {
            ReplacePokemons(Pokemons.ToList());
        }
    }

    private IEnumerable<PokemonDetails> SortPokemons(IEnumerable<PokemonDetails> pokemons)
    {
        return (CurrentSortField.Value, CurrentSortDirection.Value) switch
        {
            (PokemonSortField.Number, SortDirection.Ascending) => pokemons.OrderBy(pokemon => pokemon.Id),
            (PokemonSortField.Number, SortDirection.Descending) => pokemons.OrderByDescending(pokemon => pokemon.Id),
            (PokemonSortField.Name, SortDirection.Ascending) => pokemons.OrderBy(pokemon => pokemon.Name, StringComparer.OrdinalIgnoreCase),
            (PokemonSortField.Name, SortDirection.Descending) => pokemons.OrderByDescending(pokemon => pokemon.Name, StringComparer.OrdinalIgnoreCase),
            _ => pokemons,
        };
    }
}
