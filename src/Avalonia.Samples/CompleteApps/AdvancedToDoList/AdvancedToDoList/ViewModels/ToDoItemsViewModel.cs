using System;
using System.Collections.ObjectModel;
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
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
    
    public ToDoItemsViewModel()
    {
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        _toDoItemsSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _toDoItems,
                SortExpressionComparer<ToDoItem>
                    .Ascending(x => x.DueDate)
                    .ThenByAscending(x => x.Title ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            .Subscribe();
        
        LoadData();
    }

    private async void LoadData()
    {
        var toDoItems = await DataBaseHelper.GetToDoItemsAsync();
        _toDoItemsSourceCache.AddOrUpdate(toDoItems);
    }
    
    private readonly SourceCache<ToDoItem, int> _toDoItemsSourceCache =
        new SourceCache<ToDoItem, int>(x => x.Id ?? -1);

    private readonly ReadOnlyObservableCollection<ToDoItem> _toDoItems;

    public ReadOnlyObservableCollection<ToDoItem> ToDoItems => _toDoItems;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteToDoItemCommand), nameof(EditToDoItemCommand))]
    public partial ToDoItem? SelectedToDoItem { get; set; }
  
    [RelayCommand]
    private async Task AddNewToDoItem()
    {
        var toDoItem = new ToDoItem()
        {
            Title = "To-Do Item"
        };
        
        await EditToDoItemAsync(toDoItem);
    }

    private bool CanEditOrDeleteToDoItem(ToDoItem? toDoItem) => toDoItem != null;
    
    /// <summary>
    /// Deletes the selected ToDoItem.
    /// </summary>
    /// <param name="toDoItem">the ToDoItem to remove</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteToDoItem))]
    private async Task DeleteToDoItemAsync(ToDoItem? toDoItem)
    {
        if (toDoItem == null)
        {
            return;
        }
        
        var result = await this.ShowOverlayDialogAsync<DialogResult>("Delete To Do Item", 
            $"Are you sure you want to delete the todo item '{toDoItem.Title}'?",
            DialogCommands.YesNoCancel);
        
        if (result == DialogResult.Yes && await toDoItem.DeleteAsync())
        {
            _toDoItemsSourceCache.Remove(toDoItem);
        }
    }
    
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteToDoItem))]
    private async Task EditToDoItemAsync(ToDoItem? toDoItem)
    {
        if (toDoItem == null)
        {
            return;
        }

        var editToDoItemViewModel = new EditToDoItemViewModel(toDoItem);
        var result = await this.ShowOverlayDialogAsync<ToDoItem>("Edit ToDo-Item", editToDoItemViewModel);

        if (result != null)
        {
            _toDoItemsSourceCache.AddOrUpdate(result);
        }
    }
}