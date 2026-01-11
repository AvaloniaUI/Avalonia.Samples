using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Messages;
using AdvancedToDoList.Models;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using DynamicData.Binding;
using SharedControls.Controls;
using SharedControls.Helper;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class ManageToDoItemsViewModel 
    : ViewModelBase, IDialogParticipant, IRecipient<UpdateDataRequest<ToDoItem>>
{
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "All properties accessed by reflection are also otherwise used, so they will not be trimmed.")]
    public ManageToDoItemsViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
        
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        var filterStringObservable = this.ObserveValue(nameof(FilterString), () => FilterString)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged()
            .Select(FilterToDoItemsByText);

        var filterIsCompletedObservable = this
            .ObserveValue(nameof(ShowAlsoCompletedItems), () => ShowAlsoCompletedItems)
            .DistinctUntilChanged()
            .Select(FilterToDoItemsByIsCompleted);

        var sortObservable = this.ObserveValue(nameof(SortExpression1), () => SortExpression1)
            .CombineLatest(
                this.ObserveValue(nameof(SortExpression2), () => SortExpression2),
                this.ObserveValue(nameof(SortExpression3), () => SortExpression3),
                (s1, s2, s3) => SortExpressionComparer<ToDoItemViewModel>
                    .Ascending(s1.SortExpression)
                    .ThenByAscending(s2.SortExpression)
                    .ThenByAscending(s3.SortExpression)
            ).Select(x => x);
        
        _toDoItemsSourceCache.Connect()
            .AutoRefresh(x => x.Progress, propertyChangeThrottle: TimeSpan.FromMilliseconds(800))
            .Filter(filterStringObservable)
            .Filter(filterIsCompletedObservable)
            .ObserveOn(syncContext)
            .SortAndBind(out _toDoItems, sortObservable)
            .Subscribe();

        _ = LoadDataAsync();
    }
    
    private async Task LoadDataAsync()
    {
        var toDoItems = await DataBaseHelper.GetToDoItemsAsync(ShowAlsoCompletedItems);
        
        _toDoItemsSourceCache.AddOrUpdate(toDoItems.Select(x => new ToDoItemViewModel(x)));
    }

    private readonly SourceCache<ToDoItemViewModel, long> _toDoItemsSourceCache =
        new SourceCache<ToDoItemViewModel, long>(x => x.Id ?? -1);

    private readonly ReadOnlyObservableCollection<ToDoItemViewModel> _toDoItems;

    public ReadOnlyObservableCollection<ToDoItemViewModel> ToDoItems => _toDoItems;

    [ObservableProperty] 
    public partial string? FilterString { get; set; }

    [ObservableProperty] 
    public partial bool ShowAlsoCompletedItems { get; set; }

    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression1 { get; set; } =
        ToDoItemsSortExpression.SortByDueDateExpression;
    
    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression2 { get; set; } =
        ToDoItemsSortExpression.SortByPriorityExpression;
    
    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression3 { get; set; } =
        ToDoItemsSortExpression.SortByTitleExpression;
    
    
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

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var previousSelectedItemId = SelectedToDoItem?.Id ?? -1;

        _toDoItemsSourceCache.Clear();
        await LoadDataAsync();

        var lookUpResult = _toDoItemsSourceCache.Lookup(previousSelectedItemId);

        Dispatcher.UIThread.Post(() =>
        {
            SelectedToDoItem = lookUpResult.HasValue
                ? lookUpResult.Value
                : null;
        });
    }

    private static Func<ToDoItemViewModel, bool> FilterToDoItemsByText(string? filterText) => item =>
    {
        // we have no filter text, so this item should be visible
        if (string.IsNullOrWhiteSpace(filterText))
            return true;

        return (item.Title?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false)
               || (item.Description?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false);
    };
    
    private static Func<ToDoItemViewModel, bool> FilterToDoItemsByIsCompleted(bool showAlsoCompletedItems) => item =>
    {
        return showAlsoCompletedItems || item.Progress < 100;
    };

    // IRecipient-Impl
    public void Receive(UpdateDataRequest<ToDoItem> message)
    {
        _ = RefreshAsync();
    }
}