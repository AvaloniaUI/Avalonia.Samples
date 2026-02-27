
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using SharedControls.Controls;
using Xunit;

namespace SharedControlsTests;

/// <summary>
/// Unit tests for the HamburgerMenu control.
/// Tests menu item selection behavior, dynamic menu updates, and responsive pane display.
/// </summary>
/// <remarks>
/// Test categories:
/// - Item selection: First item selection and selection persistence
/// - Dynamic updates: Menu item collection changes and selection updates
/// - Responsive behavior: Auto-collapsing pane based on window width
/// 
/// Why test HamburgerMenu?
/// - Ensures the navigation menu behaves correctly
/// - Validates selection logic for menu items
/// - Confirms responsive UI behavior based on screen size
/// - Tests dynamic menu updates without UI corruption
/// </remarks>
public class HamburgerMenuTests
{
    private static Window CreateWindow(object content)
    {
        var window = new Window
        {
            Content = content
        };
        window.Show();
        return window;
    }

    /// <summary>
    /// Tests that the first valid menu item is automatically selected when items are added.
    /// Verifies that selection works correctly even with headers and separators present.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - HamburgerMenu initializes without a selected item
    /// - Adding items to MenuItems collection triggers selection logic
    /// - First non-header, non-separator item becomes the selected item
    /// - Loaded event processing is complete before checking selection
    /// 
    /// Test setup includes:
    /// - A header item (should be ignored for selection)
    /// - A separator item (should be ignored for selection)
    /// - The actual menu item (should be selected)
    /// 
    /// Dispatcher.RunJobs() ensures all pending UI operations complete before assertions.
    /// </remarks>
    [AvaloniaFact]
    public void AddingFirstItem_SetsAsSelected()
    {
        var hamburgerMenu = new HamburgerMenu();
        CreateWindow(hamburgerMenu);

        var item = new HamburgerMenuItem { Label = "First" };
        hamburgerMenu.MenuItems.Add(new HamburgerMenuHeaderItem { Label = "Header" });
        hamburgerMenu.MenuItems.Add(new HamburgerMenuSeparatorItem());
        hamburgerMenu.MenuItems.Add(item);

        // this is needed to make sure the Loaded-event has been processed.
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(item, hamburgerMenu.SelectedMenuItem);
    }

    /// <summary>
    /// Tests that menu selection updates correctly when items are removed.
    /// Verifies that removing the selected item shifts selection to the next valid item.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Initial state: First added item is selected by default
    /// - After removal: If the selected item is removed, selection shifts to the new first item
    /// - Selection maintains validity (never null when items exist)
    /// - UI updates properly without selection state corruption
    /// 
    /// This test ensures the menu handles:
    /// - Dynamic menu item removal
    /// - Selection preservation during collection changes
    /// - Proper indexing when menu items change
    /// 
    /// Dispatcher.RunJobs() ensures all UI updates complete before verification.
    /// </remarks>
    [AvaloniaFact]
    public void ChangingMenuItems_ReplacesSelected()
    {
        var hamburgerMenu = new HamburgerMenu();
        CreateWindow(hamburgerMenu);

        var first = new HamburgerMenuItem { Label = "First" };
        var second = new HamburgerMenuItem { Label = "Second" };
    
        hamburgerMenu.MenuItems.Add(first);
        hamburgerMenu.MenuItems.Add(second);
        Dispatcher.UIThread.RunJobs();
    
        // Initially, first is selected
        Assert.Equal(first, hamburgerMenu.SelectedMenuItem);
    
        // Remove the first item – selected should now be the new first
        hamburgerMenu.MenuItems.Remove(first);
    
        Dispatcher.UIThread.RunJobs();
    
        Assert.Equal(second, hamburgerMenu.SelectedMenuItem);
    }

    /// <summary>
    /// Tests that the hamburger pane automatically opens when window width exceeds the threshold.
    /// Verifies the responsive behavior based on the AutoClosePaneThreshold property.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Pane starts closed by default (responsive design)
    /// - When window width > AutoClosePaneThreshold → pane opens automatically
    /// - Provides more space for menu items on larger screens
    /// - Improves usability on tablets/desktops vs. mobile
    /// 
    /// Test scenario:
    /// - Threshold set to 600 pixels
    /// - Window width increased to 800 pixels (above threshold)
    /// - Pane should automatically open (IsPaneOpen = true)
    /// 
    /// This enables adaptive UI that responds to available screen real estate.
    /// Dispatcher.RunJobs() ensures layout updates complete before verification.
    /// </remarks>
    [AvaloniaFact]
    public void AutoCollapse_PaneOpens_WhenWidthAboveThreshold()
    {
        var hamburgerMenu = new HamburgerMenu
        {
            AutoClosePaneThreshold = 600
        };

        var window = CreateWindow(hamburgerMenu);
    
        // Trigger width change > threshold
        window.Width = 800;
        Dispatcher.UIThread.RunJobs();
    
        Assert.True(hamburgerMenu.IsPaneOpen);
    }

    /// <summary>
    /// Tests that the hamburger pane automatically closes when window width drops below the threshold.
    /// Verifies the reverse responsive behavior from the width-above-threshold scenario.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Pane starts open when initialized with IsPaneOpen = true
    /// - When window width is smaller than AutoClosePaneThreshold → pane closes automatically
    /// - Preserves screen space on smaller screens/mobile devices
    /// - Maintains consistent responsive behavior
    /// 
    /// Test scenario:
    /// - Threshold set to 600 pixels
    /// - Pane explicitly opened before test
    /// - Window width decreased to 400 pixels (below threshold)
    /// - Pane should automatically close (IsPaneOpen = false)
    /// 
    /// This ensures the UI adapts appropriately when switching from desktop to mobile layouts.
    /// Dispatcher.RunJobs() ensures layout recalculations complete before verification.
    /// </remarks>
    [AvaloniaFact]
    public void AutoCollapse_PaneCloses_WhenWidthBelowThreshold()
    {
        var menu = new HamburgerMenu
        {
            AutoClosePaneThreshold = 600,
            IsPaneOpen = true
        };

        var window = CreateWindow(menu);
    
        // Trigger width change < threshold
        window.Width = 400;
        Dispatcher.UIThread.RunJobs();
    
        Assert.False(menu.IsPaneOpen);
    }
}