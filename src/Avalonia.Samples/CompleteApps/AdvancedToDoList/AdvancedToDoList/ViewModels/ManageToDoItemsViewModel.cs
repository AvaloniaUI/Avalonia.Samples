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

/// <summary>
/// ViewModel for managing the display and manipulation of ToDoItems.
/// Handles filtering, sorting, CRUD operations, and real-time updates of ToDoItems.
/// Implements reactive data binding using DynamicData for efficient UI updates.
/// </summary>
[UnconditionalSuppressMessage("Trimming", "IL2112", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "We have all needed members added via DynamicallyAccessedMembers-Attribute")]
public partial class ManageToDoItemsViewModel 
    : ViewModelBase, IDialogParticipant, 
        IRecipient<UpdateDataMessage<Category>>, IRecipient<UpdateDataMessage<ToDoItem>>
{
    public ManageToDoItemsViewModel()
    {
        // Register for message notifications from other ViewModels
        WeakReferenceMessenger.Default.Register<UpdateDataMessage<Category>>(this);
        WeakReferenceMessenger.Default.Register<UpdateDataMessage<ToDoItem>>(this);
        
        // Get the current synchronization context for UI thread operations
        var syncContext = SynchronizationContext.Current  ?? throw new InvalidOperationException("No SynchronizationContext provided.");

        // Create reactive observable for text filtering with 300ms throttle to reduce frequent updates
        var filterStringObservable = this.ObserveValue(nameof(FilterString), () => FilterString)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged()
            .Select(FilterToDoItemsByText);

        // Create reactive observable for completed items filtering
        var filterIsCompletedObservable = this
            .ObserveValue(nameof(ShowAlsoCompletedItems), () => ShowAlsoCompletedItems)
            .DistinctUntilChanged()
            .Select(FilterToDoItemsByIsCompleted);

        // Create a reactive observable for sorting with a three-level sort priority
        var sortObservable = this.ObserveValue(nameof(SortExpression1), () => SortExpression1)
            .CombineLatest(
                this.ObserveValue(nameof(SortExpression2), () => SortExpression2),
                this.ObserveValue(nameof(SortExpression3), () => SortExpression3),
                (s1, s2, s3) => SortExpressionComparer<ToDoItemViewModel>
                    .Ascending(s1.SortExpression)
                    .ThenByAscending(s2.SortExpression)
                    .ThenByAscending(s3.SortExpression)
            ).Select(x => x);
        
        // Set up a reactive data pipeline: auto-refresh progress changes, apply filters and sorting
        _toDoItemsSourceCache.Connect()
            .AutoRefresh(
                x => x.Progress, 
                propertyChangeThrottle: TimeSpan.FromMilliseconds(500))
            .Filter(filterStringObservable)
            .Filter(filterIsCompletedObservable)
            .ObserveOn(syncContext)
            .SortAndBind(out _toDoItems, sortObservable)
            .Subscribe();

        // Load initial data from database
        _ = LoadDataAsync();
    }
    
    /// <summary>
    /// Loads ToDoItems from the database and populates the source cache.
    /// Creates ToDoItemViewModel wrappers for each database item.
    /// </summary>
    private async Task LoadDataAsync()
    {
        var toDoItems = await DatabaseHelper.GetToDoItemsAsync(ShowAlsoCompletedItems);
        
        // Convert database items to ViewModels and add to cache
        _toDoItemsSourceCache.AddOrUpdate(toDoItems.Select(x => new ToDoItemViewModel(x)));
    }

    /// <summary>
    /// Source cache for managing ToDoItemViewModel instances with reactive updates.
    /// Uses the ToDoItem ID as the key for efficient lookups and updates.
    /// </summary>
    private readonly SourceCache<ToDoItemViewModel, long> _toDoItemsSourceCache =
        new SourceCache<ToDoItemViewModel, long>(x => x.Id ?? -1);

    /// <summary>
    /// Read-only collection bound to the UI for displaying filtered and sorted ToDoItems.
    /// Automatically updated through the reactive pipeline.
    /// </summary>
    private readonly ReadOnlyObservableCollection<ToDoItemViewModel> _toDoItems;

    /// <summary>
    /// Public property exposing the filtered and sorted ToDoItems collection for UI binding.
    /// </summary>
    public ReadOnlyObservableCollection<ToDoItemViewModel> ToDoItems => _toDoItems;

    /// <summary>
    /// Filter text for searching ToDoItems by title or description.
    /// Changes trigger reactive filtering with 300ms throttle.
    /// </summary>
    [ObservableProperty] 
    public partial string? FilterString { get; set; }

    /// <summary>
    /// Determines whether completed items (100% progress) should be displayed.
    /// When false, only active items are shown.
    /// </summary>
    [ObservableProperty] 
    public partial bool ShowAlsoCompletedItems { get; set; }

    /// <summary>
    /// Primary sort expression for ToDoItems (defaults to due date).
    /// Changes trigger immediate re-sorting of the displayed items.
    /// </summary>
    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression1 { get; set; } =
        ToDoItemsSortExpression.SortByDueDateExpression;
    
    /// <summary>
    /// Secondary sort expression for ToDoItems (defaults to priority).
    /// Applied when items have equal values in SortExpression1.
    /// </summary>
    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression2 { get; set; } =
        ToDoItemsSortExpression.SortByPriorityExpression;
    
    /// <summary>
    /// Tertiary sort expression for ToDoItems (defaults to title).
    /// Applied when items have equal values in both SortExpression1 and SortExpression2.
    /// </summary>
    [ObservableProperty]
    public partial ToDoItemsSortExpression SortExpression3 { get; set; } =
        ToDoItemsSortExpression.SortByTitleExpression;
    
    /// <summary>
    /// Currently selected ToDoItem in the UI.
    /// Automatically enables/disables Edit and Delete commands when changed.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteToDoItemCommand), nameof(EditToDoItemCommand))]
    public partial ToDoItemViewModel? SelectedToDoItem { get; set; }

    /// <summary>
    /// Creates a new ToDoItem with default values and opens the edit dialog.
    /// The new item is only saved if the user confirms the edit dialog.
    /// </summary>
    [RelayCommand]
    private async Task AddNewToDoItem()
    {
        // Create new ToDoItem with default title
        var toDoItem = new ToDoItem()
        {
            Title = "To-Do Item"
        };

        // Open the edit dialog for the new item
        await EditToDoItemAsync(new ToDoItemViewModel(toDoItem));
    }

    /// <summary>
    /// Determines whether Edit and Delete commands can be executed.
    /// Commands are only enabled when a ToDoItem is selected.
    /// </summary>
    /// <param name="toDoItem">The ToDoItem to check</param>
    /// <returns>True if the item is not null, otherwise false</returns>
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

    /// <summary>
    /// Opens the edit dialog for the selected ToDoItem.
    /// Loads available categories and creates a clone for editing to prevent direct modification.
    /// Updates the source cache only if the user confirms changes.
    /// </summary>
    /// <param name="toDoItem">The ToDoItem to edit</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteToDoItem))]
    private async Task EditToDoItemAsync(ToDoItemViewModel? toDoItem)
    {
        if (toDoItem == null)
        {
            return;
        }

        // Load all available categories from database
        var availableCategories = await DatabaseHelper.GetCategoriesAsync();

        // Create edit ViewModel with cloned item and available categories
        var editToDoItemViewModel = new EditToDoItemViewModel(toDoItem.CloneToDoItemViewModel(),
            availableCategories.Select(x => new CategoryViewModel(x)).ToList());
        
        // Show edit dialog and wait for user confirmation
        var result = await this.ShowOverlayDialogAsync<ToDoItemViewModel>("Edit ToDo-Item", editToDoItemViewModel);

        // Update the cache if user confirmed changes
        if (result != null)
        {
            _toDoItemsSourceCache.AddOrUpdate(result);
        }
    }

    /// <summary>
    /// Refreshes the ToDoItems list by reloading from the database.
    /// Preserves the currently selected item if it still exists after refresh.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        // Remember the currently selected item ID
        var previousSelectedItemId = SelectedToDoItem?.Id ?? -1;

        // Clear cache and reload data from database
        _toDoItemsSourceCache.Clear();
        await LoadDataAsync();

        // Try to restore the previously selected item
        var lookUpResult = _toDoItemsSourceCache.Lookup(previousSelectedItemId);

        // Update selection on UI thread
        Dispatcher.UIThread.Post(() =>
        {
            SelectedToDoItem = lookUpResult.HasValue
                ? lookUpResult.Value
                : null;
        });
    }

    /// <summary>
    /// Creates a filter function for text-based filtering of ToDoItems.
    /// Searches in both title and description fields using case-insensitive comparison.
    /// </summary>
    /// <param name="filterText">The text to search for (null/empty shows all items)</param>
    /// <returns>Filter function for use with DynamicData</returns>
    private static Func<ToDoItemViewModel, bool> FilterToDoItemsByText(string? filterText) => item =>
    {
        // No filter text means this item should be visible
        if (string.IsNullOrWhiteSpace(filterText))
            return true;

        // Search filter text in title and description (case-insensitive)
        return (item.Title?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false)
               || (item.Description?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false);
    };
    
    /// <summary>
    /// Creates a filter function for hiding/showing completed ToDoItems.
    /// When showAlsoCompletedItems is false, only items with less than 100% progress are shown.
    /// </summary>
    /// <param name="showAlsoCompletedItems">Whether to include completed items</param>
    /// <returns>Filter function for use with DynamicData</returns>
    private static Func<ToDoItemViewModel, bool> FilterToDoItemsByIsCompleted(bool showAlsoCompletedItems) => item =>
    {
        return showAlsoCompletedItems || item.Progress < 100;
    };

    /// <summary>
    /// Implementation of IRecipient for receiving UpdateDataRequest messages.
    /// Automatically updates the ToDoItem's Category when data changes are requested by other ViewModels.
    /// </summary>
    /// <param name="message">The update request message</param>
    public void Receive(UpdateDataMessage<Category> message)
    {
        var updatedCategories = message.ItemsAffected;
        
        switch (message.Action)
        {
            case UpdateAction.Added:
            case UpdateAction.Updated:
                // Loop over all updated Categories
                foreach (var category in updatedCategories)
                {
                    // Loop over all ToDoItems and update the category if changed.
                    foreach (var item in _toDoItemsSourceCache.Items.Where(x => x.Category.Id == category.Id))
                    {
                        item.Category = new CategoryViewModel(category);
                    }
                }
                break;
            
            case UpdateAction.Removed:
                // Loop over all updated Categories
                foreach (var category in updatedCategories)
                {
                    // Loop over all ToDoItems and set Category to empty if it was changed. 
                    foreach (var item in _toDoItemsSourceCache.Items.Where(x => x.Category.Id == category.Id))
                    {
                        item.Category = CategoryViewModel.Empty;
                    }
                }
                break;
            
            case UpdateAction.Reset:
                _ = RefreshAsync();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        _ = RefreshAsync();
    }
    
    public void Receive(UpdateDataMessage<ToDoItem> message)
    {
        var updatedItems = message.ItemsAffected;
        
        switch (message.Action)
        {
            case UpdateAction.Added:
            case UpdateAction.Updated:
                _toDoItemsSourceCache.AddOrUpdate(updatedItems.Select(x => new ToDoItemViewModel(x)));
                break;
            
            case UpdateAction.Removed:
                _toDoItemsSourceCache.Remove(updatedItems.Select(x => new ToDoItemViewModel(x)));
                break;
            
            case UpdateAction.Reset:
                _ = RefreshAsync();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        _ = RefreshAsync();
    }
}