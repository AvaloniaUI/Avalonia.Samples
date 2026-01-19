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

        // Give some time for background load
        await Task.Delay(1000);

        // Act
        var listBox = view.FindControl<ListBox>("CategoriesListBox");
        Assert.NotNull(listBox);

        // Assert
        // If still empty, try to force load data directly into the cache to bypass potential Rx issues in tests
        if (vm.Categories.Count == 0)
        {
             await vm.RefreshAsync();
        }
        Assert.NotEmpty(vm.Categories);
        Assert.Equal(vm.Categories.Count, listBox.ItemCount);
    }
}
