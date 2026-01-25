using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;
using Xunit.Abstractions;

namespace AdvancedToDoListTests.ViewModelTests;

public class ManageToDoItemsViewModelTest : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ManageToDoItemsViewModelTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [AvaloniaFact]
    public async Task ManageToDoItemsViewModel_Constructor_InitializesCorrectly()
    {
        // Act
        var vm = new ManageToDoItemsViewModel();

        // Make sure Dispatcher related tasks have been processed
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);
        
        // Assert
        Assert.NotNull(vm.ToDoItems);
        Assert.Null(vm.FilterString);
        Assert.False(vm.ShowAlsoCompletedItems);
        Assert.Equal(ToDoItemsSortExpression.SortByDueDateExpression, vm.SortExpression1);
        Assert.Equal(ToDoItemsSortExpression.SortByPriorityExpression, vm.SortExpression2);
        Assert.Equal(ToDoItemsSortExpression.SortByTitleExpression, vm.SortExpression3);
    }

    [AvaloniaFact]
    public async Task ManageToDoItemsViewModel_Properties_CanBeSet()
    {
        // Arrange
        var vm = new ManageToDoItemsViewModel();
        
        // Make sure Dispatcher related tasks have been processed
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);
        
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
