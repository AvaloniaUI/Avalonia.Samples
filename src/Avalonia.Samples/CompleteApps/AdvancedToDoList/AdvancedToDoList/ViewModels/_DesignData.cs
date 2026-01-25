using System.Linq;
using AdvancedToDoList.Helper;

namespace AdvancedToDoList.ViewModels;

public static class _DesignData
{
    static _DesignData()
    {
        var categories = DatabaseHelper.GetCategoriesAsync().Result;
        EditCategoryViewModel = new EditCategoryViewModel(new CategoryViewModel(categories.First()));
        
        var toDoItems = DatabaseHelper.GetToDoItemsAsync().Result;
        EditToDoItemViewModel = new EditToDoItemViewModel(new ToDoItemViewModel(toDoItems.First()), categories.Select(x => new CategoryViewModel(x)).ToList());
    }
    
    public static EditCategoryViewModel EditCategoryViewModel { get; }
    
    public static EditToDoItemViewModel EditToDoItemViewModel { get; }
}