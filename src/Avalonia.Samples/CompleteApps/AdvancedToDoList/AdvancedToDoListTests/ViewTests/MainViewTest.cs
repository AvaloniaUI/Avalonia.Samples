using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

/// <summary>
/// Unit tests for the MainView control.
/// Tests initialization and HamburgerMenu integration to ensure the main UI component loads correctly.
/// </summary>
/// <remarks>
/// Test focus:
/// - Verifies that MainView initializes with the expected HamburgerMenu control
/// - Confirms the menu structure: 2 main menu items and 1 options menu item
/// - Validates the view-model binding and view instantiation flow
/// </remarks>
public class MainViewTest : TestBase
{
    /// <summary>
    /// Tests that MainView initializes correctly with the expected HamburgerMenu structure.
    /// Verifies the presence and basic configuration of the main navigation menu.
    /// </summary>
    /// <remarks>
    /// This test ensures:
    /// - MainView is created with the correct data context
    /// - HamburgerMenu named "MainMenu" is present in the view
    /// - The Main menu contains 2 items
    /// - The Options menu contains 1 item
    /// 
    /// If any of these assertions fail, the UI structure may have changed unexpectedly.
    /// </remarks>
    [AvaloniaFact]
    public async Task MainView_Should_Initialize_With_HamburgerMenu()
    {
        // Arrange
        var vm = new MainViewModel();
        var view = new MainView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var hamburgerMenu = view.FindControl<HamburgerMenu>("MainMenu");
        
        // Wait for the UI to load
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(100);
        
        // Assert
        Assert.NotNull(hamburgerMenu);
        Assert.Equal(2, hamburgerMenu.MenuItems.Count);
        Assert.Single(hamburgerMenu.OptionsMenuItems);
    }
}
