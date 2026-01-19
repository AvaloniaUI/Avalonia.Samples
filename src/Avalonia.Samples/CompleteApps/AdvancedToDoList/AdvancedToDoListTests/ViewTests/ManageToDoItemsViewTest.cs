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

        // Give some time for background load
        await Task.Delay(1000);

        // Act
        var listBox = view.FindControl<ListBox>("ToDoItemsListBox");
        Assert.NotNull(listBox);

        // Assert
        if (vm.ToDoItems.Count == 0)
        {
            await vm.RefreshAsync();
        }
        Assert.NotEmpty(vm.ToDoItems);
        Assert.Equal(vm.ToDoItems.Count, listBox.ItemCount);
    }
}
