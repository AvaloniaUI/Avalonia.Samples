using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Headless.XUnit;
using Xunit;
using Avalonia.Controls;
using AdvancedToDoListTests.Services;
using SharedControls.Controls;

namespace AdvancedToDoListTests.ViewTests;

/// <summary>
/// Unit tests for EditCategoryView.
/// Tests UI data binding between the view and EditCategoryViewModel,
/// including TextBox text updates and SaveButton command execution.
/// </summary>
/// <remarks>
/// Why test EditCategoryView?
/// - Validates that XAML bindings (TwoWay) work correctly in headless mode
/// - Confirms ViewModel updates when user types in the UI
/// - Tests command binding (SaveCommand and CancelCommand) integration
/// - Ensures view-model communication works in real UI scenarios
/// 
/// Test categories covered:
/// - Data binding: TextBox text changes propagate to ViewModel
/// - Command binding: SaveButton triggers SaveCommand correctly
/// - Headless UI: Avalonia controls work without visible window
/// - Integration: View + ViewModel + service mocks work together
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Headless Avalonia window setup for UI tests
/// - Control.FindControl() to access named UI elements
/// - Mock object pattern for services (IDialogService, ICategoryService)
/// </remarks>
public class EditCategoryViewTest : TestBase
{
    /// <summary>
    /// Tests that when a user types text into the NameTextBox,
    /// the underlying ViewModel's Item.Name property is updated.
    /// Verifies TwoWay data binding works correctly in the view.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - TextBox Text property binds to ViewModel.Item.Name
    /// - Typing in the TextBox updates the ViewModel property
    /// - The View-model remains the source of truth for data
    /// 
    /// Why this matters:
    /// - Ensures user input flows correctly to the ViewModel
    /// - Validates XAML binding syntax is correct
    /// - Confirms TwoWay binding mode works in headless tests
    /// </remarks>
    [AvaloniaFact]
    public void EditCategoryView_Should_Update_ViewModel_When_TextBox_Changes()
    {
        // Arrange - Create view with initial ViewModel state
        var category = new CategoryViewModel { Name = "Initial Name" };
        var vm = new EditCategoryViewModel(category);
        var view = new EditCategoryView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act - Update the TextBox text (simulating user typing)
        var nameTextBox = view.FindControl<TextBox>("NameTextBox");
        Assert.NotNull(nameTextBox);

        nameTextBox.Text = "Updated Name";

        // Assert - Verify ViewModel property was updated
        Assert.Equal("Updated Name", vm.Item.Name);
    }

    /// <summary>
    /// Tests that clicking the SaveButton correctly executes the SaveCommand,
    /// which saves the category via the ICategoryService and returns a dialog result.
    /// Verifies command binding and service integration.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - SaveButton's Command property binds to ViewModel.SaveCommand
    /// - Executing the command triggers category save operation
    /// - ICategoryService.SaveCategoryAsync is called with correct data
    /// - Dialog service receives the save result
    /// 
    /// Why this matters:
    /// - Confirms UI buttons trigger correct ViewModel logic
    /// - Validates service integration in UI context
    /// - Ensures dialog flow completes successfully
    /// </remarks>
    [AvaloniaFact]
    public void SaveButton_Should_Trigger_SaveCommand()
    {
        // Arrange - Set up view with mocks for deterministic testing
        var category = new CategoryViewModel { Name = "Test" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);
        var view = new EditCategoryView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act - Execute the SaveButton's command
        var saveButton = view.FindControl<Button>("SaveButton");

        Assert.NotNull(saveButton);
        saveButton.Command?.Execute(null);

        // Assert - Verify save operation was performed
        Assert.True(mockCategoryService.SavedCategory != null);
        Assert.Equal("Test", mockCategoryService.SavedCategory.Name);
    }

    /// <summary>
    /// Tests that validation errors are displayed when the user tries to save
    /// with invalid data (e.g., empty required Name field).
    /// Verifies that the UI prevents saving and shows appropriate error messages.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Validation errors are detected before saving
    /// - SaveButton is disabled or SaveCommand doesn't execute
    /// - ValidationSummary or inline error UI shows the error
    /// - User is prevented from saving invalid data
    /// 
    /// Why this matters:
    /// - Ensures data integrity by preventing invalid categories
    /// - Provides clear feedback to users about what needs correction
    /// - Validates data annotation attributes ([Required]) work in UI
    /// </remarks>
    [AvaloniaFact]
    public void ValidationErrors_Should_Display_When_Name_Is_Empty()
    {
        // Arrange - Start with invalid data (empty required field)
        var category = new CategoryViewModel();
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);
        var view = new EditCategoryView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Verify initial validation state
        Assert.False(vm.Item.HasErrors);
        Assert.False(vm.Item.GetErrors(nameof(vm.Item.Name)).Any());

        // Mimic invalid input
        var nameTextBox = view.FindControl<TextBox>("NameTextBox");
        nameTextBox!.Text = "";

        // Verify the correct validation state
        Assert.True(vm.Item.HasErrors);
        Assert.True(vm.Item.GetErrors(nameof(vm.Item.Name)).Any());

        // Act - Try to execute save with invalid data
        var saveButton = view.FindControl<Button>("SaveButton");
        Assert.NotNull(saveButton);

        // Act - Attempt to save (command should not execute due to validation)
        saveButton.Command?.Execute(null);

        // Assert - Verify save was prevented and errors are shown
        Assert.Null(mockCategoryService.SavedCategory);
        Assert.True(vm.Item.HasErrors);
    }

    /// <summary>
    /// Tests that clicking the CancelButton handles all user choices correctly:
    /// - Yes: Saves and returns the category
    /// - No: Does not save, returns null
    /// - Cancel: Does not save, dialog stays open
    /// </summary>
    /// <param name="userChoice">
    /// The dialog button which the user clicked.
    /// </param>
    /// <param name="expectSave">
    /// Whether the category should be saved to the service.
    /// </param>
    /// <param name="expectDialogClosed">
    /// Whether the dialog should close (true for Yes/No, false for Cancel).
    /// </param>
    /// <param name="expectResultType">
    /// The expected result type from the dialog (null for No/Cancel, CategoryViewModel for Yes).
    /// </param>
    [AvaloniaTheory]
    [InlineData(DialogResult.Yes, true, true, typeof(CategoryViewModel))]
    [InlineData(DialogResult.No, false, true, null)]
    [InlineData(DialogResult.Cancel, false, false, null)]
    public void CancelButton_Should_Handle_UserChoice_Correctly(
        DialogResult userChoice,
        bool expectSave,
        bool expectDialogClosed,
        object? expectResultType)
    {
        // Arrange
        var originalName = "Original Name";
        var category = new CategoryViewModel { Name = originalName };
        var mockDialog = new MockDialogService { NextDialogResult = userChoice };
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);

        vm.Item.Name = "Modified Name";

        var view = new EditCategoryView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var cancelButton = view.FindControl<Button>("CancelButton");
        Assert.NotNull(cancelButton);
        cancelButton.Command?.Execute(null);

        // Assert
        Assert.Equal(expectSave, mockCategoryService.SavedCategory is not null);
        Assert.Equal(expectDialogClosed, mockDialog.ReturnResultCalled);
        Assert.Equal(expectResultType, mockDialog.ReturnedResult?.GetType());
    }

    /// <summary>
    /// Tests that saving a valid category updates the ViewModel and returns
    /// the correct data to the dialog service.
    /// Verifies the complete save flow with valid input.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Valid data passes validation
    /// - ICategoryService.SaveCategoryAsync is called
    /// - Dialog service receives the saved category
    /// - ViewModel reflects any post-save changes
    /// </remarks>
    [AvaloniaFact]
    public void SaveCommand_WithValidData_Should_ReturnResultAndCloseDialog()
    {
        // Arrange - Start with valid data
        var category = new CategoryViewModel { Name = "Valid Category" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);

        // Ensure no validation errors
        Assert.False(vm.Item.HasErrors);

        var view = new EditCategoryView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act - Save valid category
        var saveButton = view.FindControl<Button>("SaveButton");
        Assert.NotNull(saveButton);
        saveButton.Command?.Execute(null);

        // Assert - Verify complete save flow
        Assert.NotNull(mockCategoryService.SavedCategory);
        Assert.Equal("Valid Category", mockCategoryService.SavedCategory.Name);
        Assert.True(mockDialog.ReturnResultCalled);
    }
}