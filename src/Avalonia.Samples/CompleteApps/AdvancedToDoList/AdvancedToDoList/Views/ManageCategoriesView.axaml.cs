using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// View for managing categories in the application.
/// Provides user interface for creating, editing, and deleting categories.
/// Supports color coding and organization of ToDoItems into logical groups.
/// </summary>
public partial class ManageCategoriesView : UserControl
{
    /// <summary>
    /// Initializes ManageCategoriesView and sets up UI components.
    /// Binds to ManageCategoriesViewModel for category management functionality.
    /// </summary>
    public ManageCategoriesView()
    {
        InitializeComponent();
    }
}