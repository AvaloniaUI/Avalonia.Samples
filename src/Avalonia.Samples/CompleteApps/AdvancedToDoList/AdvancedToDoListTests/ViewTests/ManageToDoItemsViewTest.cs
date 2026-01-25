using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

public class ManageToDoItemsViewTest : TestBase
{
    [AvaloniaFact]
    public async Task ManageToDoItemsView_Should_Display_Items()
    {
        // Arrange
        var vm = new ManageToDoItemsViewModel();
        var view = new ManageToDoItemsView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Give some time for view initialization
        await Task.Delay(100);

        // Act
        var listBox = view.FindControl<ListBox>("ToDoItemsListBox");

        // Assert
        Assert.NotNull(listBox);
        Assert.Equal(vm.ToDoItems.Count, listBox.ItemCount);
    }
}
