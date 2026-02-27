using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// Dialog view for editing individual ToDoItems.
/// Provides form interface for modifying title, description, priority, due date, and category.
/// Supports validation and real-time preview of ToDoItem changes.
/// Used in overlay dialogs for both creating new and editing existing ToDoItems.
/// </summary>
public partial class EditToDoItemView : UserControl
{
    /// <summary>
    /// Initializes EditToDoItemView and sets up form controls.
    /// Binds to EditToDoItemViewModel for validation and data management.
    /// </summary>
    public EditToDoItemView()
    {
        InitializeComponent();
    }
}