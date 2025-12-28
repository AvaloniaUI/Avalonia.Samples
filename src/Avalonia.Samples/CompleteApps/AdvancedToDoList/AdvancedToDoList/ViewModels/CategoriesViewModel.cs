using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class CategoriesViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
    
    public CategoriesViewModel()
    {
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        _categoriesSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _categories,
                SortExpressionComparer<CategoryViewModel>
                    .Ascending(x => x.Name ?? string.Empty)
                    .ThenByAscending(x => x.Id ?? -1))
            .Subscribe();
        
        LoadData();
    }

    private async void LoadData()
    {
        var categories = await DataBaseHelper.GetCategoriesAsync();
        _categoriesSourceCache.AddOrUpdate(categories.Select(x => new CategoryViewModel(x)));
    }
    
    private readonly SourceCache<CategoryViewModel, int> _categoriesSourceCache =
        new SourceCache<CategoryViewModel, int>(c => c.Id ?? -1);

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

        var categoryViewModel = new EditCategoryViewModel(category);
        var result = await this.ShowOverlayDialogAsync<CategoryViewModel>("Add a new category", categoryViewModel);

        if (result != null)
        {
            _categoriesSourceCache.AddOrUpdate(result);
        }
    }
}