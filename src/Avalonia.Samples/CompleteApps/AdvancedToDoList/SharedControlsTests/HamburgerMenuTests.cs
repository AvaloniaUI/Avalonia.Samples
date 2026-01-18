using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using SharedControls.Controls;
using Xunit;

namespace SharedControlsTests;

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

    [AvaloniaFact]
    public void AddingFirstItem_SetsAsSelected()
    {
        var hamburgerMenu = new HamburgerMenu();
        CreateWindow(hamburgerMenu);

        var item = new HamburgerMenuItem { Label = "First" };
        hamburgerMenu.MenuItems.Add(new HamburgerMenuHeaderItem {Label = "Header"});
        hamburgerMenu.MenuItems.Add(new HamburgerMenuSeparatorItem());
        hamburgerMenu.MenuItems.Add(item);

        // this is needed to make sure the Loaded-event has beenm processed.
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(item, hamburgerMenu.SelectedMenuItem);
    }

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