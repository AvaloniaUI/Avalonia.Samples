using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Models;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using SharedControls.Controls;
using SharedControls.Helper;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase, IDialogParticipant
{
    public ToDoItemsViewModel()
    {
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        var filterObservable = this.ObserveValue(nameof(FilterString), () => FilterString)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged()
            .Select(FilterToDoItemsByText);
        
        _toDoItemsSourceCache.Connect()
            .ObserveOn(syncContext)
            .Filter(filterObservable)
            .SortAndBind(out _toDoItems,
                SortExpressionComparer<ToDoItemViewModel>
                    .Ascending(x => x.DueDate)
                    .ThenByAscending(x => x.Title ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            .Subscribe();
        
        LoadData();
    }

    private async void LoadData()
    {
        var toDoItems = await DataBaseHelper.GetToDoItemsAsync();
        _toDoItemsSourceCache.AddOrUpdate(toDoItems.Select(x => new ToDoItemViewModel(x)));
    }
    
    private readonly SourceCache<ToDoItemViewModel, int> _toDoItemsSourceCache =
        new SourceCache<ToDoItemViewModel, int>(x => x.Id ?? -1);

    private readonly ReadOnlyObservableCollection<ToDoItemViewModel> _toDoItems;

    public ReadOnlyObservableCollection<ToDoItemViewModel> ToDoItems => _toDoItems;

    [ObservableProperty]
    public partial string? FilterString {get; set;}
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteToDoItemCommand), nameof(EditToDoItemCommand))]
    public partial ToDoItemViewModel? SelectedToDoItem { get; set; }
  
    [RelayCommand]
    private async Task AddNewToDoItem()
    {
        var toDoItem = new ToDoItem()
        {
            Title = "To-Do Item"
        };
        
        await EditToDoItemAsync(new ToDoItemViewModel(toDoItem));
    }

    private bool CanEditOrDeleteToDoItem(ToDoItemViewModel? toDoItem) => toDoItem != null;
    
    /// <summary>
    /// Deletes the selected ToDoItem.
    /// </summary>
    /// <param name="toDoItem">the ToDoItem to remove</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteToDoItem))]
    private async Task DeleteToDoItemAsync(ToDoItemViewModel? toDoItem)
    {
        if (toDoItem == null)
        {
            return;
        }
        
        var result = await this.ShowOverlayDialogAsync<DialogResult>("Delete To Do Item", 
            $"Are you sure you want to delete the todo item '{toDoItem.Title}'?",
            DialogCommands.YesNoCancel);
        
        if (result == DialogResult.Yes && await toDoItem.ToToDoItem().DeleteAsync())
        {
            _toDoItemsSourceCache.Remove(toDoItem);
        }
    }
    
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteToDoItem))]
    private async Task EditToDoItemAsync(ToDoItemViewModel? toDoItem)
    {
        if (toDoItem == null)
        {
            return;
        }

        var availableCategories = await DataBaseHelper.GetCategoriesAsync();
        
        var editToDoItemViewModel = new EditToDoItemViewModel(toDoItem.CloneToDoItemViewModel(), 
            availableCategories.Select(x => new CategoryViewModel(x)).ToList());
        var result = await this.ShowOverlayDialogAsync<ToDoItemViewModel>("Edit ToDo-Item", editToDoItemViewModel);

        if (result != null)
        {
            _toDoItemsSourceCache.AddOrUpdate(result);
        }
    }
    
    private static Func<ToDoItemViewModel, bool> FilterToDoItemsByText(string? filterText) => item =>
    {
        // we have no filter text, so this item should be visible
        if (string.IsNullOrWhiteSpace(filterText))
            return true;
        
        return (item.Title?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Description?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false);
    };
}