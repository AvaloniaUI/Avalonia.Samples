using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

/// <summary>
/// Unit tests for the ManageCategoriesView control.
/// Tests category display functionality and list binding to ensure
/// the category management interface renders correctly.
/// </summary>
/// <remarks>
/// Test focus:
/// - Verifies that ManageCategoriesView displays the category list properly
/// - Confirms the CategoriesListBox is present and bound to the ViewModel
/// - Validates the relationship between ViewModel data and UI presentation
/// 
/// Why test ManageCategoriesView?
/// - Ensures category list UI renders correctly
/// - Validates data binding between ViewModel and View
/// - Confirms theListBox control is accessible for user interaction
/// </remarks>
public class ManageCategoriesViewTest : TestBase
{
    /// <summary>
    /// Tests that ManageCategoriesView displays categories from the ViewModel.
    /// Verifies the list box is present and contains the expected number of items.
    /// </summary>
    /// <remarks>
    /// This test ensures:
    /// - ManageCategoriesView is created with the correct data context
    /// - CategoriesListBox is present in the view with the correct name
    /// - The list item count matches the ViewModel's category count
    /// - The UI properly reflects the underlying data model
    /// 
    /// Note: A small delay (100ms) is included to allow for asynchronous
    /// UI initialization before testing control states.
    /// </remarks>
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

        // Give some time for view initialization
        await Task.Delay(100);

        // Act
        var listBox = view.FindControl<ListBox>("CategoriesListBox");

        // Assert
        Assert.NotNull(listBox);
        Assert.Equal(vm.Categories.Count, listBox.ItemCount);
    }
}
