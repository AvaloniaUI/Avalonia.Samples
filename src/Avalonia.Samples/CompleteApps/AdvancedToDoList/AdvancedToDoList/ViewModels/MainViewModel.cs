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

namespace AdvancedToDoList.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
    
    public MainViewModel()
    {
        var syncContext = SynchronizationContext.Current ?? new AvaloniaSynchronizationContext();

        _categoriesSourceCache.Connect()
            .ObserveOn(syncContext)
            .SortAndBind(out _categories,
                SortExpressionComparer<CategoryViewModel>
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
        _categoriesSourceCache.AddOrUpdate(categories.Select(x => new CategoryViewModel(x)));
    }
    
    private readonly SourceCache<CategoryViewModel, int> _categoriesSourceCache =
        new SourceCache<CategoryViewModel, int>(c => c.Id ?? -1);

    private readonly ReadOnlyObservableCollection<CategoryViewModel> _categories;

    public ReadOnlyObservableCollection<CategoryViewModel> Categories => _categories;

    
    private readonly SourceCache<ToDoItemViewModel, int> _toDoItemsSourceCache =
        new SourceCache<ToDoItemViewModel, int>(i => i.Id ?? -1);
    
    private readonly ReadOnlyObservableCollection<ToDoItemViewModel> _toDoItems;
    
    public ReadOnlyObservableCollection<ToDoItemViewModel> ToDoItems => _toDoItems;

    [RelayCommand]
    private async Task AddNewCategory()
    {
        var categoryViewModel = new CategoryViewModel()
        {
            Name = "New Category",
            Description = "This is a new category",
            GroupColor = ColorHelper.GetRandomColor()
        };

        await categoryViewModel.SaveAsync();
        
        _categoriesSourceCache.AddOrUpdate(categoryViewModel);
    }
}