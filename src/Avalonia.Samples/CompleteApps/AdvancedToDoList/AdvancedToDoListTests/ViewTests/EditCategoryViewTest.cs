using System.Threading.Tasks;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Headless.XUnit;
using Xunit;
using Avalonia.VisualTree;
using System.Linq;
using Avalonia.Controls;
using AdvancedToDoListTests.ViewModelTests;
using AdvancedToDoList.Services;
using SharedControls.Controls;

namespace AdvancedToDoListTests.ViewTests;

public class EditCategoryViewTest : TestBase
{
    [AvaloniaFact]
    public async Task EditCategoryView_Should_Update_ViewModel_When_TextBox_Changes()
    {
        // Arrange
        var category = new CategoryViewModel { Name = "Initial Name" };
        var vm = new EditCategoryViewModel(category);
        var view = new EditCategoryView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var nameTextBox = view.FindControl<TextBox>("NameTextBox");
        Assert.NotNull(nameTextBox);

        nameTextBox.Text = "Updated Name";

        // Assert
        Assert.Equal("Updated Name", vm.Item.Name);
    }

    [AvaloniaFact]
    public async Task SaveButton_Should_Trigger_SaveCommand()
    {
         // Arrange
        var category = new CategoryViewModel { Name = "Test" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);
        var view = new EditCategoryView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var saveButton = view.FindControl<Button>("SaveButton");
        
        Assert.NotNull(saveButton);
        saveButton.Command.Execute(null);

        // Assert
        Assert.True(mockCategoryService.SavedCategory != null);
        Assert.Equal("Test", mockCategoryService.SavedCategory.Name);
    }

    private class MockDialogService : IDialogService
    {
        public Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands) => Task.FromResult(default(T));
        public void ReturnResultFromOverlayDialog(object? result) { }
    }

    private class MockCategoryService : ICategoryService
    {
        public AdvancedToDoList.Models.Category? SavedCategory { get; private set; }
        public Task<bool> SaveCategoryAsync(AdvancedToDoList.Models.Category category)
        {
            SavedCategory = category;
            return Task.FromResult(true);
        }
        public Task<bool> DeleteCategoryAsync(AdvancedToDoList.Models.Category category) => Task.FromResult(true);
    }
}
