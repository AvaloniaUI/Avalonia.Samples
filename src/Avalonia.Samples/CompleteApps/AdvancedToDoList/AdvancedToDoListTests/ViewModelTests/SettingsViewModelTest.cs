using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class SettingsViewModelTest : TestBase
{
    [Fact]
    public void SettingsViewModel_Constructor_InitializesCorrectly()
    {
        // Act
        var vm = new SettingsViewModel();

        // Assert
        Assert.NotNull(vm.Settings);
        Assert.NotEmpty(vm.AvailableThemeVariants);
        Assert.Contains("Default", vm.AvailableThemeVariants);
        Assert.Contains("Dark", vm.AvailableThemeVariants);
        Assert.Contains("Light", vm.AvailableThemeVariants);
    }
}
