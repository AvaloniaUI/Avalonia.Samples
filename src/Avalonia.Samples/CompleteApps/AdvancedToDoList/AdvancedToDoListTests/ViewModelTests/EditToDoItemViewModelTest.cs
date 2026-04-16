
using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using AdvancedToDoListTests.Services;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for EditToDoItemViewModel.
/// Tests all aspects of EditToDoItemViewModel including initialization,
/// category management, save operations with success and validation error scenarios,
/// and proper dialog integration.
/// </summary>
/// <remarks>
/// Why test EditToDoItemViewModel?
/// - Validates ViewModel behavior independently of the UI
/// - Ensures proper category assignment including the special "Uncategorized" option
/// - Tests save operations with success/failure handling
/// - Verifies validation error showDialog integration
/// - Confirms dialog service integration
/// 
/// Test categories covered:
/// - Constructor behavior: Initialization with item and categories
/// - Category management: Setting category to empty (uncategorized)
/// - Save operations: Successful save and validation error scenarios
/// - Dialog integration: Proper showDialog display for errors
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Mock object pattern for dependencies (IDialogService, IToDoService)
/// - Independent test isolation (no shared state)
/// </remarks>
public class EditToDoItemViewModelTest
{
    /// <summary>
    /// Tests that the constructor properly initializes the ViewModel with item and categories.
    /// Verifies the to-do item is wrapped and the available categories list is populated correctly
    /// with the "Uncategorized" option as the first entry.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Item property references the provided to-do item ViewModel
    /// - AvailableCategories list includes "Uncategorized" as the first entry
    /// - Provided categories are appended after the empty category
    /// </remarks>
    [Fact]
    public void EditToDoItemViewModel_Constructor_InitializesCorrectly()
    {
        // Arrange - Prepare a to-do item and categories list
        var toDoItem = new ToDoItemViewModel(new ToDoItem { Title = "Test" });
        var categories = new List<CategoryViewModel> 
        { 
            new CategoryViewModel { Name = "Category 1" }, 
            new CategoryViewModel { Name = "Category 2" } 
        };

        // Act - Create the ViewModel with the to-do item and categories
        var vm = new EditToDoItemViewModel(toDoItem, categories);

        // Assert - Verify item reference and categories list structure
        Assert.Same(toDoItem, vm.Item);
        Assert.Equal(3, vm.AvailableCategories.Count);
        Assert.Same(CategoryViewModel.Empty, vm.AvailableCategories[0]);
        Assert.Equal("Category 1", vm.AvailableCategories[1].Name);
        Assert.Equal("Category 2", vm.AvailableCategories[2].Name);
    }

    /// <summary>
    /// Tests that setting the category to empty properly updates the to-do item's category.
    /// Verifies the command correctly assigns the "Uncategorized" category.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Item's Category property is set to CategoryViewModel.Empty
    /// - Command works without throwing exceptions
    /// </remarks>
    [Fact]
    public void EditToDoItemViewModel_SetCategoryToEmpty_UpdatesItemCategory()
    {
        // Arrange - Create a to-do item with an initial category
        var toDoItem = new ToDoItemViewModel(new ToDoItem { Title = "Test" });
        var categories = new List<CategoryViewModel>();
        var vm = new EditToDoItemViewModel(toDoItem, categories);
        vm.Item.Category = new CategoryViewModel { Name = "Some Category" };

        // Act - Execute the command to set the category to empty
        vm.SetCategoryToEmptyCommand.Execute(null);

        // Assert - Verify the category was changed to "Uncategorized"
        Assert.Same(CategoryViewModel.Empty, vm.Item.Category);
    }

    /// <summary>
    /// Tests that the SaveCommand successfully saves the to-do item and closes the dialog.
    /// Verifies proper integration with the service and dialog system.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Service SaveToDoItemAsync is called with the correct to-do item
    /// - the dialog result is returned to close the dialog
    /// - the returned result contains the saved to-do item data
    /// </remarks>
    [Fact]
    public async Task SaveCommand_Success_ReturnsResultAndClosesDialog()
    {
        // Arrange - Set up a test to-do item and mocks
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Test Task" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoItemService { SaveResult = true };
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);

        // Act - Execute the SaveCommand
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert - Verify the service was called, the dialog result was returned, and data matches
        Assert.NotNull(mockToDoService.SavedItem);
        Assert.Equal("Test Task", mockToDoService.SavedItem.Title);
        Assert.True(mockDialog.ReturnResultCalled);
        Assert.IsType<ToDoItemViewModel>(mockDialog.ReturnedResult);
        Assert.Equal("Test Task", ((ToDoItemViewModel)mockDialog.ReturnedResult!).Title);
    }

    /// <summary>
    /// Tests that validation errors show an error dialog and don't save.
    /// Verifies proper validation handling and error showDialog integration.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Validation errors are detected before saving
    /// - Error showDialog is shown with correct title
    /// - the dialog result is not returned (dialog stays open)
    /// </remarks>
    [Fact]
    public async Task SaveCommand_ValidationError_ShowsErrorDialog()
    {
        // Arrange - Start with valid data, then break validation
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Valid" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoItemService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);

        // Trigger validation error by setting Title to null and revalidating
        vm.Item.Title = null;
        vm.Item.Validate();

        // Act - Try to save despite the invalid state
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert - Verify validation error state, dialog shown, and no result returned
        Assert.True(vm.Item.HasErrors);
        Assert.Equal("Error", mockDialog.LastTitle);
        Assert.False(mockDialog.ReturnResultCalled);
    }
}