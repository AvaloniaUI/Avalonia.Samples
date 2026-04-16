using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// Dialog view for editing categories.
/// Provides a form interface for modifying category name, description, and color.
/// Includes color picker and validation controls.
/// Used in overlay dialogs for both creating new and editing existing categories.
/// </summary>
public partial class EditCategoryView : UserControl
{
    /// <summary>
    /// Initializes EditCategoryView and sets up form controls.
    /// Binds to EditCategoryViewModel for validation and data management.
    /// </summary>
    public EditCategoryView()
    {
        InitializeComponent();
    }
}