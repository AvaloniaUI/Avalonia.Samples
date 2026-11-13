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
    private const int DefaultPageSize = 25;
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
    [NotifyCanExecuteChangedFor(nameof(RemoveFilterCommand))]
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages))]
    [NotifyPropertyChangedFor(nameof(HasNextPage))]
    private int _totalPokemonCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreviousPage))]
    [NotifyPropertyChangedFor(nameof(HasNextPage))]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages))]
    [NotifyPropertyChangedFor(nameof(HasNextPage))]
    private int _currentPageSize = DefaultPageSize;

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

    public IReadOnlyList<int> PageSizeOptions { get; } = [25, 50, 100];

    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalPokemonCount / CurrentPageSize));

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task SearchAsync()
    {
        string pokemonName = PokemonName.Trim();
        PokemonName = pokemonName;

        CurrentPage = 1;
        await LoadPokemonsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task ClearFilters()
    {
        PokemonName = string.Empty;
        CurrentPokemonType = null;
        CurrentPokemonGeneration = null;
        CurrentPage = 1;
        await LoadPokemonsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
    private async Task RemoveFilterAsync(PokemonFilterKind filter)
    {
        switch (filter)
        {
            case PokemonFilterKind.Name:
                PokemonName = string.Empty;
                break;
            case PokemonFilterKind.Type:
                CurrentPokemonType = null;
                break;
            case PokemonFilterKind.Generation:
                CurrentPokemonGeneration = null;
                break;
        }

        CurrentPage = 1;
        await LoadPokemonsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private async Task GoToPreviousPageAsync()
    {
        CurrentPage--;
        await LoadPokemonsAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private async Task GoToNextPageAsync()
    {
        CurrentPage++;
        await LoadPokemonsAsync();
    }

    public async Task LoadPokemonsAsync()
    {
        if (IsLoading)
        {
            return;
        }

        Pokemons.Clear();
        TotalPokemonCount = 0;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            PokemonSearchResult response = await _pokeApiClient.SearchPokemonsAsync(
                PokemonName,
                CurrentPokemonType?.Name,
                CurrentPokemonGeneration?.Name,
                (CurrentPage - 1) * CurrentPageSize,
                CurrentPageSize);

            TotalPokemonCount = response.TotalCount;
            ReplacePokemons(response.Pokemons);

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

    private bool CanGoToPreviousPage()
    {
        return !IsLoading && HasPreviousPage;
    }

    private bool CanGoToNextPage()
    {
        return !IsLoading && HasNextPage;
    }

    partial void OnIsLoadingChanged(bool value)
    {
        GoToPreviousPageCommand.NotifyCanExecuteChanged();
        GoToNextPageCommand.NotifyCanExecuteChanged();
    }

    private void ReplacePokemons(IEnumerable<PokemonDetails> pokemons)
    {
        Pokemons.Clear();

        foreach (PokemonDetails pokemon in SortPokemons(pokemons))
        {
            Pokemons.Add(pokemon);
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
