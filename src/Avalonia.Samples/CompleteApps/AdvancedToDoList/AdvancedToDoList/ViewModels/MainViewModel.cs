using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

/// <summary>
/// This is our MainViewModel.
/// </summary>
public class MainViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Gets the ViewModel that manages the categories.
    /// </summary>
    public ManageCategoriesViewModel CategoriesViewModel { get; } = new();
    
    /// <summary>
    /// Gets the ViewModel that manages the to-do-items.
    /// </summary>
    public ManageToDoItemsViewModel ToDoItemsViewModel { get; } = new();
    
    /// <summary>
    /// Gets the ViewModel that handles the App-settings.
    /// </summary>
    public SettingsViewModel SettingsViewModel { get; } = new();
}