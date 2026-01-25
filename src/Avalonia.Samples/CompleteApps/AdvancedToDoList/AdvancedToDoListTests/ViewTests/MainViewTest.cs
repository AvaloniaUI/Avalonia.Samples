using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

public class MainViewTest : TestBase
{
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

        // Assert
        Assert.NotNull(hamburgerMenu);
        Assert.Equal(2, hamburgerMenu.MenuItems.Count);
        Assert.Equal(1, hamburgerMenu.OptionsMenuItems.Count);
    }
}
