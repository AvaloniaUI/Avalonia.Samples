using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// View for managing ToDoItems with filtering, sorting, and CRUD operations.
/// Provides user interface for creating, editing, deleting, and organizing ToDoItems.
/// Includes search functionality, progress tracking, and category management.
/// </summary>
public partial class ManageToDoItemsView : UserControl
{
    /// <summary>
    /// Initializes ManageToDoItemsView and sets up UI components.
    /// Binds to ManageToDoItemsViewModel for data and functionality.
    /// </summary>
    public ManageToDoItemsView()
    {
        InitializeComponent();
    }
}