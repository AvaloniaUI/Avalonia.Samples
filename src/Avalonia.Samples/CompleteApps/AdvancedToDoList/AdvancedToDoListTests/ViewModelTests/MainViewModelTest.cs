using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class MainViewModelTest : TestBase
{
    [AvaloniaFact]
    public async Task MainViewModel_Constructor_InitializesChildViewModels()
    {
        // Act
        var vm = new MainViewModel();

        // Make sure Dispatcher related tasks have been processed
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);

        // Assert
        Assert.NotNull(vm.CategoriesViewModel);
        Assert.NotNull(vm.ToDoItemsViewModel);
        Assert.NotNull(vm.SettingsViewModel);
    }
}
