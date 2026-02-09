using System.Linq;
using AdvancedToDoList.Helper;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// Provides design-time data for the Avalonia previewer and designer.
/// This class is only used during design time to display meaningful sample data
/// in visual designers and previews, making UI development easier.
/// </summary>
public static class _DesignData
{
    /// <summary>
    /// Static constructor that initializes design-time data.
    /// Loads real data from the database to provide realistic examples
    /// for the visual designer to display.
    /// </summary>
    static _DesignData()
    {
        // Load categories from database for design-time preview
        var categories = DatabaseHelper.GetCategoriesAsync().Result;
        EditCategoryViewModel = new EditCategoryViewModel(new CategoryViewModel(categories.First()));
        
        // Load ToDoItems and create corresponding ViewModel for design-time preview
        var toDoItems = DatabaseHelper.GetToDoItemsAsync().Result;
        EditToDoItemViewModel = new EditToDoItemViewModel(new ToDoItemViewModel(toDoItems.First()), categories.Select(x => new CategoryViewModel(x)).ToList());
    }
    
    /// <summary>
    /// Gets the design-time data instance for the EditCategoryView.
    /// Used by the visual designer to display a realistic category editing interface.
    /// </summary>
    public static EditCategoryViewModel EditCategoryViewModel { get; }
    
    /// <summary>
    /// Gets the design-time data instance for the EditToDoItemView.
    /// Used by the visual designer to display a realistic ToDo item editing interface
    /// with sample categories available for selection.
    /// </summary>
    public static EditToDoItemViewModel EditToDoItemViewModel { get; }
}