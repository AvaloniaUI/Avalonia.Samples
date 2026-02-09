using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// Main application window that serves as the top-level container for the To-Do-Application.
/// Hosts the main user interface and manages application lifecycle events.
/// Provides the primary frame for all user interactions within the application.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes MainWindow and sets up the application's primary window.
    /// This constructor is called during application startup to create the main interface.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }
}