
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using AdvancedToDoList.ViewModels;
using AdvancedToDoListTests.Services;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for EditCategoryViewModel.
/// Tests all aspects of EditCategoryViewModel including initialization,
/// save operations with success and validation error scenarios, and
/// proper dialog integration.
/// </summary>
/// <remarks>
/// Why test EditCategoryViewModel?
/// - Validates ViewModel behavior independently of the UI
/// - Ensures proper save operations with success/failure handling
/// - Tests validation error showDialog integration
/// - Verifies dialog service integration
/// 
/// Test categories covered:
/// - Constructor behavior: Default constructor and constructor with item
/// - Save operations: Successful save and validation error scenarios
/// - Dialog integration: Proper showDialog display for errors
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Mock object pattern for dependencies (IDialogService, ICategoryService)
/// - Independent test isolation (no shared state)
/// </remarks>
public class EditCategoryViewModelTest
{
    /// <summary>
    /// Tests that creating a new EditCategoryViewModel without parameters creates a new item.
    /// Verifies the default constructor initializes Item with default values.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Item is created (not null)
    /// - Item.Id is null (indicating it's a new item not yet saved to the database)
    /// </remarks>
    [Fact]
    public void EditCategoryViewModel_DefaultConstructor_CreatesNewItem()
    {
        // Arrange & Act - Create a new instance of the ViewModel
        var vm = new EditCategoryViewModel();

        // Assert - Verify Item is created and has no ID (new item)
        Assert.NotNull(vm.Item);
        Assert.Null(vm.Item.Id);
    }

    /// <summary>
    /// Tests that providing a CategoryViewModel to the constructor uses that item.
    /// Verifies the ViewModel properly wraps the provided item instead of creating a copy.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Same reference is used (not a copy)
    /// - Changes to the ViewModel's Item affect the original item
    /// </remarks>
    [Fact]
    public void EditCategoryViewModel_ConstructorWithItem_UsesProvidedItem()
    {
        // Arrange - Prepare a CategoryViewModel to pass into the constructor
        var category = new CategoryViewModel();
        
        // Act - Create the ViewModel with the category
        var vm = new EditCategoryViewModel(category);

        // Assert - Verify the same reference is used (not a copy)
        Assert.Same(category, vm.Item);
    }

    /// <summary>
    /// Tests that the SaveCommand successfully saves the category and closes the dialog.
    /// Verifies proper integration with the service and dialog system.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Service SaveCategoryAsync is called with the correct category
    /// - A Dialog result is returned to close the dialog
    /// - The returned result contains the saved category data
    /// </remarks>
    [Fact]
    public async Task SaveCommand_Success_ReturnsResultAndClosesDialog()
    {
        // Arrange - Set up a test category and mocks
        var category = new CategoryViewModel { Name = "Test Category" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService { SaveResult = true };
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);

        // Act - Execute the SaveCommand
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert - Verify the service was called, dialog result was returned, and data matches
        Assert.NotNull(mockCategoryService.SavedCategory);
        Assert.Equal("Test Category", mockCategoryService.SavedCategory.Name);
        Assert.True(mockDialog.ReturnResultCalled);
        Assert.IsType<CategoryViewModel>(mockDialog.ReturnedResult);
        Assert.Equal("Test Category", ((CategoryViewModel)mockDialog.ReturnedResult!).Name);
    }

    /// <summary>
    /// Tests that validation errors show an error dialog and don't save.
    /// Verifies proper validation handling and error showDialog integration.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Validation errors are detected before saving
    /// - Error showDialog is shown with correct title and message
    /// - A dialog result is not returned (dialog stays open)
    /// </remarks>
    [Fact]
    public async Task SaveCommand_ValidationError_ShowsErrorDialog()
    {
        // Arrange - Start with valid data, then break validation
        var category = new CategoryViewModel { Name = "Valid" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);
    
        // Trigger validation error by setting Name to null and revalidating
        vm.Item.Name = null; 
        vm.Item.Validate();

        // Act - Try to save despite invalid state
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert - Verify validation error state, dialog shown, and no result returned
        Assert.True(vm.Item.HasErrors);
        Assert.Equal("Error", mockDialog.LastTitle);
        Assert.Equal("Please correct the errors in the form.", mockDialog.LastContent);
        Assert.False(mockDialog.ReturnResultCalled);
    }
}