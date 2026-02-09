using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// Main application view that displays the primary user interface.
/// Contains navigation between categories, ToDo items, and settings sections.
/// Serves as the root container for the entire application UI.
/// </summary>
public partial class MainView : UserControl
{
    /// <summary>
    /// Initializes the MainView and sets up the user interface components.
    /// This constructor is called when the main view is created by the framework.
    /// </summary>
    public MainView()
    {
        InitializeComponent();
    }
}