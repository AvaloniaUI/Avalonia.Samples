using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

/// <summary>
/// Unit tests for the ManageToDoItemsView control.
/// Tests To-Do item display functionality and list binding to ensure
/// the task management interface renders correctly.
/// </summary>
/// <remarks>
/// Test focus:
/// - Verifies that ManageToDoItemsView displays the To-Do list properly
/// - Confirms the ToDoItemsListBox is present and bound to the ViewModel
/// - Validates the relationship between ViewModel data and UI presentation
/// 
/// Why test ManageToDoItemsView?
/// - Ensures task list UI renders correctly
/// - Validates data binding between ViewModel and View
/// - Confirms the ListBox control is accessible for user interaction
/// </remarks>
public class ManageToDoItemsViewTest : TestBase
{
    /// <summary>
    /// Tests that ManageToDoItemsView displays To-Do items from the ViewModel.
    /// Verifies the list box is present and contains the expected number of items.
    /// </summary>
    /// <remarks>
    /// This test ensures:
    /// - ManageToDoItemsView is created with correct data context
    /// - ToDoItemsListBox is present in the view with correct name
    /// - The list item count matches the ViewModel's To-Do item count
    /// - The UI properly reflects the underlying data model
    /// 
    /// Note: A small delay (100ms) is included to allow for asynchronous
    /// UI initialization before testing control states.
    /// </remarks>
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