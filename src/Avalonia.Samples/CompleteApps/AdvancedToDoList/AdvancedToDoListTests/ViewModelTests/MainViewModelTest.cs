using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class MainViewModelTest : TestBase
{
    [Fact]
    public async Task MainViewModel_Constructor_InitializesChildViewModels()
    {
        // Act
        var vm = new MainViewModel();

        // Give some time for background LoadDataAsync to start/run
        await Task.Delay(100);

        // Assert
        Assert.NotNull(vm.CategoriesViewModel);
        Assert.NotNull(vm.ToDoItemsViewModel);
        Assert.NotNull(vm.SettingsViewModel);
    }
}
