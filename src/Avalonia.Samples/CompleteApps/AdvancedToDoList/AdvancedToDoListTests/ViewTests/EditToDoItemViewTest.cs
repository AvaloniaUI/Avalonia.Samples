using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewTests;

public class EditToDoItemViewTest : TestBase
{
    [AvaloniaFact]
    public async Task EditToDoItemView_Should_Update_ViewModel_When_TextBox_Changes()
    {
        // Arrange
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Initial Title" });
        var categories = new List<CategoryViewModel>();
        var vm = new EditToDoItemViewModel(item, categories);
        var view = new EditToDoItemView
        {
            DataContext = vm
        };
        var window = new Window { Content = view };
        window.Show();

        // Act
        var titleTextBox = view.FindControl<TextBox>("TitleTextBox");
        Assert.NotNull(titleTextBox);

        titleTextBox.Text = "Updated Title";

        // Assert
        Assert.Equal("Updated Title", vm.Item.Title);
    }

    [AvaloniaFact]
    public async Task SaveButton_Should_Trigger_SaveCommand()
    {
        // Arrange
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Test Task" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);
        var view = new EditToDoItemView
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
        Assert.NotNull(mockToDoService.SavedItem);
        Assert.Equal("Test Task", mockToDoService.SavedItem.Title);
    }

    private class MockDialogService : IDialogService
    {
        public Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands) => Task.FromResult(default(T));
        public void ReturnResultFromOverlayDialog(object? result) { }
    }

    private class MockToDoService : IToDoService
    {
        public ToDoItem? SavedItem { get; private set; }
        public Task<bool> SaveToDoItemAsync(ToDoItem item)
        {
            SavedItem = item;
            return Task.FromResult(true);
        }
        public Task<bool> DeleteToDoItemAsync(ToDoItem item) => Task.FromResult(true);
    }
}
