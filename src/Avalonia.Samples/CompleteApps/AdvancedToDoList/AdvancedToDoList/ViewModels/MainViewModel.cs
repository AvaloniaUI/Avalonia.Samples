using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// Main ViewModel that orchestrates the entire application's ViewModels.
/// Acts as the root container for all major application sections and coordinates navigation
/// between Categories, ToDoItems, and Settings. Implements IDialogParticipant to support
/// dialog interactions throughout the application.
/// </summary>
public class MainViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Gets the ViewModel that manages the Categories collection and CRUD operations.
    /// Provides functionality for creating, editing, deleting, and organizing Categories.
    /// </summary>
    public ManageCategoriesViewModel CategoriesViewModel { get; } = new();
    
    /// <summary>
    /// Gets the ViewModel that manages the ToDoItems collection and their CRUD operations.
    /// Handles filtering, sorting, editing, and managing ToDoItems across categories.
    /// </summary>
    public ManageToDoItemsViewModel ToDoItemsViewModel { get; } = new();
    
    /// <summary>
    /// Gets the ViewModel that handles application settings and configuration.
    /// Provides access to theme settings, data import/export, and database management.
    /// </summary>
    public SettingsViewModel SettingsViewModel { get; } = new();
}