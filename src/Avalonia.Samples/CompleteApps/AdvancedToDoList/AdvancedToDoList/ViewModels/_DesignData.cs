using System.Linq;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Views;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This class is only used by the previewer to display some meaningful data.
/// </summary>
public static class _DesignData
{
    static _DesignData()
    {
        var categories = DatabaseHelper.GetCategoriesAsync().Result;
        EditCategoryViewModel = new EditCategoryViewModel(new CategoryViewModel(categories.First()));
        
        var toDoItems = DatabaseHelper.GetToDoItemsAsync().Result;
        EditToDoItemViewModel = new EditToDoItemViewModel(new ToDoItemViewModel(toDoItems.First()), categories.Select(x => new CategoryViewModel(x)).ToList());
    }
    
    /// <summary>
    /// Gets the design data for the <see cref="EditCategoryView"/>
    /// </summary>
    public static EditCategoryViewModel EditCategoryViewModel { get; }
    
    /// <summary>
    /// Gets the design data for the <see cref="EditToDoItemView"/>
    /// </summary>
    public static EditToDoItemViewModel EditToDoItemViewModel { get; }
}