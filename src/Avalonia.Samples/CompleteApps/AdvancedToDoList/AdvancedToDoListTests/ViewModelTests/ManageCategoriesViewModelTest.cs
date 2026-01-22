using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class ManageCategoriesViewModelTest : TestBase
{
    [AvaloniaFact]
    public async Task ManageCategoriesViewModel_Constructor_InitializesCorrectly()
    {
        // Act
        // Note: This might trigger LoadDataAsync which depends on DataBaseHelper
        var vm = new ManageCategoriesViewModel();

        // Make sure Dispatcher related tasks have been processed
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);
        
        // Assert
        Assert.NotNull(vm.Categories);
        Assert.Null(vm.SelectedCategory);
    }

    [AvaloniaFact]
    public async Task ManageCategoriesViewModel_SelectedCategory_CanBeSet()
    {
        // Arrange
        var vm = new ManageCategoriesViewModel();
        var category = new CategoryViewModel();

        // Make sure Dispatcher related tasks have been processed
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);
        
        // Act
        vm.SelectedCategory = category;

        // Assert
        Assert.Same(category, vm.SelectedCategory);
    }
}
