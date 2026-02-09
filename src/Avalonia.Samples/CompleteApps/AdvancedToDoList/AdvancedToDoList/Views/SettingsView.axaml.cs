using Avalonia.Controls;

namespace AdvancedToDoList.Views;

/// <summary>
/// Settings configuration view for application preferences and data management.
/// Provides interface for theme selection, data import/export, and database operations.
/// Supports application-wide configuration settings and data backup/restore functionality.
/// </summary>
public partial class SettingsView : UserControl
{
    /// <summary>
    /// Initializes SettingsView and sets up configuration controls.
    /// Binds to SettingsViewModel for managing application settings.
    /// </summary>
    public SettingsView()
    {
        InitializeComponent();
    }
}