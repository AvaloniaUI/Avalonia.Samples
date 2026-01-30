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
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This ViewModel will manage all of our categories.
/// </summary>
public partial class ManageCategoriesViewModel 
    : ViewModelBase, IDialogParticipant, IRecipient<UpdateDataRequest<Category>>
{
    /// <summary>
    /// Creates a new instance of this ViewModel.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Handled via rd.xml")]
    public ManageCategoriesViewModel()
    {
        // Registers ourselves to the default WeakReferenceMessenger which is needed to receive the UpdateDataRequest-messaged. 
        WeakReferenceMessenger.Default.Register(this);
        
        // This sets up the source cache. Please check https://github.com/reactivemarbles/DynamicData for full documentation.
        _categoriesSourceCache.Connect()
            // Make sure to observe the changes on the correct SynchronizationContext, since Avalonia requires that 
            // change events are sent from the UI-Thread. 
            .ObserveOn(SynchronizationContext.Current) 
            // We sort by categories name. If two items have the same name, we then sort by its ID.
            .SortAndBind(out _categories,
                SortExpressionComparer<CategoryViewModel>
                    .Ascending(x => x.Name ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            // Remember to subscribe to the changes, otherwise the UI will never update. 
            .Subscribe();

        // In the constructor, we don't want to wait for the data to be loaded. This can happen in the background. 
        // Some more info about fire-and-forget calls: https://techcommunity.microsoft.com/blog/educatordeveloperblog/fire-and-forget-methods-in-c-%E2%80%94-best-practices--pitfalls/4299605
        _ = LoadDataAsync();
    }

    /// <summary>
    /// This task will load the data async.
    /// </summary>
    private async Task LoadDataAsync()
    {
        // fetch all categories
        var categories = await DatabaseHelper.GetCategoriesAsync();
        
        // Use AddOrUpdate, which will update existing items and add new items.
        _categoriesSourceCache.AddOrUpdate(categories.Select(x => new CategoryViewModel(x)));
    }
    
    // This is the SourceCache, which stores all of our items.
    private readonly SourceCache<CategoryViewModel, long> _categoriesSourceCache =
        new SourceCache<CategoryViewModel, long>(c => c.Id ?? -1);

    /// <summary>
    /// This is the backing field for the <see cref="Categories"/>-collection.
    /// </summary>
    private readonly ReadOnlyObservableCollection<CategoryViewModel> _categories;

    /// <summary>
    /// Gets a read-only collection of all loaded categories.
    /// </summary>
    public ReadOnlyObservableCollection<CategoryViewModel> Categories => _categories;

    /// <summary>
    /// Gets or sets the selected category. 
    /// </summary>
    /// <remarks>When this changes, also the edit and delete command should be notified.</remarks>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCategoryCommand), nameof(EditCategoryCommand))]
    public partial CategoryViewModel? SelectedCategory { get; set; }

    /// <summary>
    /// Gets a command that adds a new Category.
    /// </summary>
    [RelayCommand]
    private async Task AddNewCategory()
    {
        var category = new CategoryViewModel();

        await EditCategoryAsync(category);
    }

    /// <summary>
    /// This method checks if the given category can be edited or deleted.
    /// </summary>
    /// <param name="category">the item that should be edited</param>
    /// <returns>true if the item is not null</returns>
    private static bool CanEditOrDeleteCategory(CategoryViewModel? category) => category != null;

    /// <summary>
    /// Gets a command that deletes the provided category.
    /// </summary>
    /// <param name="category">the category to remove</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCategory))]
    private async Task DeleteCategoryAsync(CategoryViewModel? category)
    {
        if (category == null)
        {
            return;
        }

        var result = await this.ShowOverlayDialogAsync<DialogResult>("Delete category",
            $"Are you sure you want to delete the category '{category.Name}'?",
            DialogCommands.YesNoCancel);

        if (result == DialogResult.Yes && await category.ToCategory().DeleteAsync())
        {
            _categoriesSourceCache.Remove(category);
        }
    }

    /// <summary>
    /// Gets a command that edits the provided category.
    /// </summary>
    /// <param name="category">The category to edit.</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCategory))]
    private async Task EditCategoryAsync(CategoryViewModel? category)
    {
        // In theory this should never be null but better to safeguard it here. 
        if (category == null)
        {
            return;
        }

        // Maje sure to clone the category, otherwise the passed item will be updated without explicitly saving it.
        var categoryViewModel = new EditCategoryViewModel((CategoryViewModel)category.Clone());
        
        // Show the dialog and wait for the result.
        var result = await this.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", categoryViewModel);

        // if the result is not null, the category was saved successfully, and we need to update our collection. 
        // Using dynamic data, it will automatically update the item if it was available with the same ID before or 
        // add it as a new item if the ID wasn't present.
        if (result != null)
        {
            _categoriesSourceCache.AddOrUpdate(result);
            SelectedCategory = category;
            
            // Notify To-Do-Items that the categories have changed and a refresh should be considered.
            // In production, we could also consider refining the message to pass the item that was actually changed and thus 
            // the update could be a bit quicker. However, this is only worth the effort if we expect a lot of items. 
            WeakReferenceMessenger.Default.Send(new UpdateDataRequest<ToDoItem>());
        }
    }
    
    /// <summary>
    /// Gets a command that refreshes the entire list.
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        // remember the ID that was selected before.
        var previousSelectedItemId = SelectedCategory?.Id ?? -1;

        // clear the source cache, then load the data again.
        _categoriesSourceCache.Clear();
        await LoadDataAsync();

        // try to find the previous selected item. and select it again.
        var lookUpResult = _categoriesSourceCache.Lookup(previousSelectedItemId);
        
        // Note: Since we are inside a task, we have to post this change on the UIThread. 
        // If you want to avoid calling the Dispatcher form the ViewModel, you can also write a helper service to do it. 
        Dispatcher.UIThread.Post(() =>
        {
            SelectedCategory = lookUpResult.HasValue
                ? lookUpResult.Value
                : null;
        });
    }

    // IRecipient-Impl

    /// <inheritdoc />
    public void Receive(UpdateDataRequest<Category> message)
    {
        _ = RefreshAsync();
    }
}