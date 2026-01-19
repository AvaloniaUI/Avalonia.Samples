using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class ManageToDoItemsViewModelTest : TestBase
{
    [Fact]
    public void ManageToDoItemsViewModel_Constructor_InitializesCorrectly()
    {
        // Act
        var vm = new ManageToDoItemsViewModel();

        // Assert
        Assert.NotNull(vm.ToDoItems);
        Assert.Null(vm.FilterString);
        Assert.False(vm.ShowAlsoCompletedItems);
        Assert.Equal(ToDoItemsSortExpression.SortByDueDateExpression, vm.SortExpression1);
        Assert.Equal(ToDoItemsSortExpression.SortByPriorityExpression, vm.SortExpression2);
        Assert.Equal(ToDoItemsSortExpression.SortByTitleExpression, vm.SortExpression3);
    }

    [Fact]
    public void ManageToDoItemsViewModel_Properties_CanBeSet()
    {
        // Arrange
        var vm = new ManageToDoItemsViewModel();

        // Act
        vm.FilterString = "Test";
        vm.ShowAlsoCompletedItems = true;
        vm.SelectedToDoItem = new ToDoItemViewModel(new() { Title = "Task" });

        // Assert
        Assert.Equal("Test", vm.FilterString);
        Assert.True(vm.ShowAlsoCompletedItems);
        Assert.NotNull(vm.SelectedToDoItem);
        Assert.Equal("Task", vm.SelectedToDoItem.Title);
    }
}
