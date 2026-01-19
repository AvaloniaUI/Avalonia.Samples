using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class ManageCategoriesViewModelTest : TestBase
{
    [Fact]
    public void ManageCategoriesViewModel_Constructor_InitializesCorrectly()
    {
        // Act
        // Note: This might trigger LoadDataAsync which depends on DataBaseHelper
        var vm = new ManageCategoriesViewModel();

        // Assert
        Assert.NotNull(vm.Categories);
        Assert.Null(vm.SelectedCategory);
    }

    [Fact]
    public void ManageCategoriesViewModel_SelectedCategory_CanBeSet()
    {
        // Arrange
        var vm = new ManageCategoriesViewModel();
        var category = new CategoryViewModel();

        // Act
        vm.SelectedCategory = category;

        // Assert
        Assert.Same(category, vm.SelectedCategory);
    }
}
