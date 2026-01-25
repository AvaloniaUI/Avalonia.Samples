using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

public class ManageCategoriesViewTest : TestBase
{
    [AvaloniaFact]
    public async Task ManageCategoriesView_Should_Display_Categories()
    {
        // Arrange
        var vm = new ManageCategoriesViewModel();
        var view = new ManageCategoriesView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Give some time for view initialization
        await Task.Delay(100);

        // Act
        var listBox = view.FindControl<ListBox>("CategoriesListBox");

        // Assert
        Assert.NotNull(listBox);
        Assert.Equal(vm.Categories.Count, listBox.ItemCount);
    }
}
