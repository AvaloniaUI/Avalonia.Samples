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

public partial class ManageCategoriesViewModel 
    : ViewModelBase, IDialogParticipant, IRecipient<UpdateDataRequest<Category>>
{
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "Handled via rd.xml")]
    public ManageCategoriesViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
        
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        _categoriesSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _categories,
                SortExpressionComparer<CategoryViewModel>
                    .Ascending(x => x.Name ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            .Subscribe();

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var categories = await DataBaseHelper.GetCategoriesAsync();
        _categoriesSourceCache.AddOrUpdate(categories.Select(x => new CategoryViewModel(x)));
    }

    private readonly SourceCache<CategoryViewModel, long> _categoriesSourceCache =
        new SourceCache<CategoryViewModel, long>(c => c.Id ?? -1);

    private readonly ReadOnlyObservableCollection<CategoryViewModel> _categories;

    public ReadOnlyObservableCollection<CategoryViewModel> Categories => _categories;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCategoryCommand), nameof(EditCategoryCommand))]
    public partial CategoryViewModel? SelectedCategory { get; set; }

    [RelayCommand]
    private async Task AddNewCategory()
    {
        var category = new CategoryViewModel();

        await EditCategoryAsync(category);
    }

    private bool CanEditOrDeleteCategory(CategoryViewModel? category) => category != null;

    /// <summary>
    /// Deletes the selected category.
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

    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCategory))]
    private async Task EditCategoryAsync(CategoryViewModel? category)
    {
        if (category == null)
        {
            return;
        }

        var categoryViewModel = new EditCategoryViewModel((CategoryViewModel)category.Clone());
        
        var result = await this.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", categoryViewModel);

        if (result != null)
        {
            _categoriesSourceCache.AddOrUpdate(result);
            
            // Notify To-Do-Items that the categories have changed and a refresh should be considered.
            // In production, we could also consider refining the message to pass the item that was actually changed and thus 
            // the update could be a bit quicker. However, this is only worth the effort if we expect a lot of items. 
            WeakReferenceMessenger.Default.Send(new UpdateDataRequest<ToDoItem>());
        }
    }
    
    [RelayCommand]
    private async Task RefreshAsync()
    {
        var previousSelectedItemId = SelectedCategory?.Id ?? -1;

        _categoriesSourceCache.Clear();
        await LoadDataAsync();

        var lookUpResult = _categoriesSourceCache.Lookup(previousSelectedItemId);

        Dispatcher.UIThread.Post(() =>
        {
            SelectedCategory = lookUpResult.HasValue
                ? lookUpResult.Value
                : null;
        });
    }

    // IRecipient-Impl
    public void Receive(UpdateDataRequest<Category> message)
    {
        _ = RefreshAsync();
    }
}