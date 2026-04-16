using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using AdvancedToDoListTests.Services;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

/// <summary>
/// Unit tests for EditToDoItemView.
/// Tests UI data binding between the view and EditToDoItemViewModel,
/// including TextBox text updates and SaveButton command execution.
/// </summary>
/// <remarks>
/// Why test EditToDoItemView?
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
/// - Mock object pattern for services (IDialogService, IToDoItemService)
/// </remarks>
public class EditToDoItemViewTest : TestBase
{
    /// <summary>
    /// Tests that when a user types text into the TitleTextBox,
    /// the underlying ViewModel's Item.Title property is updated.
    /// Verifies TwoWay data binding works correctly in the view.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - TextBox Text property binds to ViewModel.Item.Title
    /// - Typing in the TextBox updates the ViewModel property
    /// - The View-model remains the source of truth for data
    /// 
    /// Why this matters:
    /// - Ensures user input flows correctly to the ViewModel
    /// - Validates XAML binding syntax is correct
    /// - Confirms TwoWay binding mode works in headless tests
    /// </remarks>
    [AvaloniaFact]
    public void EditToDoItemView_Should_Update_ViewModel_When_TextBox_Changes()
    {
        // Arrange - Create view with initial ViewModel state
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Initial Title" });
        var categories = new List<CategoryViewModel>();
        var vm = new EditToDoItemViewModel(item, categories);
        var view = new EditToDoItemView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act - Update the TextBox text (simulating user typing)
        var titleTextBox = view.FindControl<TextBox>("TitleTextBox");
        Assert.NotNull(titleTextBox);

        titleTextBox.Text = "Updated Title";

        // Assert - Verify ViewModel property was updated
        Assert.Equal("Updated Title", vm.Item.Title);
    }

    /// <summary>
    /// Tests that clicking the SaveButton correctly executes the SaveCommand,
    /// which saves the to-do item via the IToDoItemService and returns a dialog result.
    /// Verifies command binding and service integration.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - SaveButton's Command property binds to ViewModel.SaveCommand
    /// - Executing the command triggers to-do item save operation
    /// - IToDoItemService.SaveItemAsync is called with correct data
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
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Test Task" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoItemService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);
        var view = new EditToDoItemView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act - Execute the SaveButton's command
        var saveButton = view.FindControl<Button>("SaveButton");
        Assert.NotNull(saveButton);
        saveButton.Command?.Execute(null);

        // Assert - Verify save operation was performed
        Assert.NotNull(mockToDoService.SavedItem);
        Assert.Equal("Test Task", mockToDoService.SavedItem.Title);
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
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Test Task" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoItemService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);
        var view = new EditToDoItemView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Verify initial validation state
        Assert.False(vm.Item.HasErrors);
        Assert.False(vm.Item.GetErrors(nameof(vm.Item.Title)).Any());

        // Mimic invalid input
        var nameTextBox = view.FindControl<TextBox>("TitleTextBox");
        nameTextBox!.Text = "";

        // Verify the correct validation state
        Assert.True(vm.Item.HasErrors);
        Assert.True(vm.Item.GetErrors(nameof(vm.Item.Title)).Any());

        // Act - Try to execute save with invalid data
        var saveButton = view.FindControl<Button>("SaveButton");
        Assert.NotNull(saveButton);

        // Act - Attempt to save (command should not execute due to validation)
        saveButton.Command?.Execute(null);

        // Assert - Verify save was prevented and errors are shown
        Assert.Null(mockToDoService.SavedItem);
        Assert.True(vm.Item.HasErrors);
    }

    /// <summary>
    /// Tests that clicking the CancelButton handles all user choices correctly:
    /// - Yes: Saves and returns the to-do item
    /// - No: Does not save, returns null
    /// - Cancel: Does not save, dialog stays open
    /// </summary>
    /// <param name="userChoice">
    /// The dialog button which the user clicked.
    ///   * DialogResult.Yes  → Save and return the to-do item
    ///   * DialogResult.No   → Discard changes and return null
    ///   * DialogResult.Cancel → Keep editing (dialog stays open)
    /// </param>
    /// <param name="expectSave">
    /// Whether the to-do item should be saved to the service.
    ///   * true  → To-do item was saved (Yes was clicked)
    ///   * false → To-do item was not saved (No or Cancel was clicked)
    /// </param>
    /// <param name="expectDialogClosed">
    /// Whether the dialog should close.
    ///   * true  → Dialog closed (Yes or No was clicked)
    ///   * false → Dialog stays open (Cancel was clicked)
    /// </param>
    /// <param name="expectResultType">
    /// The expected result type from the dialog.
    ///   * typeof(ToDoItemViewModel) → ToDoItemViewModel returned (Yes was clicked)
    ///   * null                       → Null returned (No was clicked) or dialog stays open (Cancel)
    /// </param>
    /// <remarks>
    /// Expected behavior:
    /// - CancelButton triggers confirmation dialog with Yes/No/Cancel options
    /// - DialogResult.Yes: Saves to-do item, returns saved to-do item, dialog closes
    /// - DialogResult.No: Does not save, returns null, dialog closes
    /// - DialogResult.Cancel: Does not save, dialog stays open
    /// 
    /// Why this matters:
    /// - Ensures users can safely exit without losing data
    /// - Prevents accidental saves when canceling
    /// - Validates dialog service cancellation flow with confirmation
    /// 
    /// Testing patterns used:
    /// - Parameterized tests to cover multiple user choices
    /// - Mock service pattern for predictable testing
    /// - Headless Avalonia window setup
    /// </remarks>
    [AvaloniaTheory]
    [InlineData(DialogResult.Yes, true, true, typeof(ToDoItemViewModel))]
    [InlineData(DialogResult.No, false, true, null)]
    [InlineData(DialogResult.Cancel, false, false, null)]
    public void CancelButton_Should_Handle_UserChoice_Correctly(
        DialogResult userChoice,
        bool expectSave,
        bool expectDialogClosed,
        object? expectResultType)
    {
        // Arrange
        var originalTitle = "Original Title";
        var item = new ToDoItemViewModel(new ToDoItem { Title = originalTitle });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService { NextDialogResult = userChoice };
        var mockToDoService = new MockToDoItemService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);

        // Modify ViewModel data to verify it's not saved (unless user chooses Yes)
        vm.Item.Title = "Modified Title";

        var view = new EditToDoItemView { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var cancelButton = view.FindControl<Button>("CancelButton");
        Assert.NotNull(cancelButton);
        cancelButton.Command?.Execute(null);

        // Assert
        Assert.Equal(expectSave, mockToDoService.SavedItem is not null);
        Assert.Equal(expectDialogClosed, mockDialog.ReturnResultCalled);
        
        // Verify result type matches expectation
        if (expectResultType is null)
        {
            Assert.Null(mockDialog.ReturnedResult);
        }
        else
        {
            Assert.NotNull(mockDialog.ReturnedResult);
            Assert.Equal(expectResultType, mockDialog.ReturnedResult!.GetType());
        }
    }
}