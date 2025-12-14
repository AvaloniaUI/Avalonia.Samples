using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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

public partial class MainViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
    
    public MainViewModel()
    {
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        _categoriesSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _categories,
                SortExpressionComparer<Category>
                    .Ascending(x => x.Name ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            .Subscribe();
        
        _toDoItemsSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _toDoItems,
                SortExpressionComparer<ToDoItemViewModel>
                    .Ascending(x => x.Priority)
                    .ThenByAscending(x => x.DueDate))
            .Subscribe();
        
        LoadData();
    }

    private async void LoadData()
    {
        var categories = await DataBaseHelper.GetCategoriesAsync();
        _categoriesSourceCache.AddOrUpdate(categories);
    }
    
    private readonly SourceCache<Category, int> _categoriesSourceCache =
        new SourceCache<Category, int>(c => c.Id ?? -1);

    private readonly ReadOnlyObservableCollection<Category> _categories;

    public ReadOnlyObservableCollection<Category> Categories => _categories;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCategoryCommand), nameof(EditCategoryCommand))]
    public partial Category? SelectedCategory { get; set; }

    private readonly SourceCache<ToDoItemViewModel, int> _toDoItemsSourceCache =
        new SourceCache<ToDoItemViewModel, int>(i => i.Id ?? -1);
    
    private readonly ReadOnlyObservableCollection<ToDoItemViewModel> _toDoItems;
    
    public ReadOnlyObservableCollection<ToDoItemViewModel> ToDoItems => _toDoItems;

    [RelayCommand]
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "DynamicData requires reflection")]
    private async Task AddNewCategory()
    {
        var category = new Category()
        {
            Name = "New Category",
            Description = "This is a new category",
            GroupColorHex = ColorHelper.GetRandomColor().ToString()
        };
        
        await EditCategoryAsync(category);
    }

    private bool CanEditOrDeleteCategory(Category? category) => category != null;
    
    /// <summary>
    /// Deletes the selected category.
    /// </summary>
    /// <param name="category">the category to remove</param>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCategory))]
    private async Task DeleteCategoryAsync(Category? category)
    {
        var result = await this.ShowOverlayDialogAsync<DialogResult>("Delete category", 
            $"Are you sure you want to delete the category '{category.Name}'?",
            DialogCommands.YesNoCancel);
        
        if (result == DialogResult.Yes && await category.DeleteAsync())
        {
            _categoriesSourceCache.Remove(category);
        }
    }
    
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteCategory))]
    private async Task EditCategoryAsync(Category? category)
    {
        if (category == null)
        {
            return;
        }

        var categoryViewModel = new EditCategoryViewModel(category);
        var result = await this.ShowOverlayDialogAsync<Category>("Add a new category", categoryViewModel);

        if (result != null)
        {
            _categoriesSourceCache.AddOrUpdate(result);
        }
    }
}