using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using AdvancedToDoList.ViewModels;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class EditToDoItemViewModelTest
{
    private class MockDialogService : IDialogService
    {
        public string? LastTitle { get; private set; }
        public object? LastContent { get; private set; }
        public object? ReturnedResult { get; private set; }
        public bool ReturnResultCalled { get; private set; }

        public Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands)
        {
            LastTitle = title;
            LastContent = content;
            return Task.FromResult(default(T));
        }

        public void ReturnResultFromOverlayDialog(object? result)
        {
            ReturnedResult = result;
            ReturnResultCalled = true;
        }
    }

    private class MockToDoService : IToDoService
    {
        public bool SaveResult { get; set; } = true;
        public ToDoItem? SavedItem { get; private set; }

        public Task<bool> SaveToDoItemAsync(ToDoItem item)
        {
            SavedItem = item;
            return Task.FromResult(SaveResult);
        }

        public Task<bool> DeleteToDoItemAsync(ToDoItem item)
        {
            return Task.FromResult(true);
        }
    }

    [Fact]
    public void EditToDoItemViewModel_Constructor_InitializesCorrectly()
    {
        // Arrange
        var toDoItem = new ToDoItemViewModel(new ToDoItem { Title = "Test" });
        var categories = new List<CategoryViewModel> 
        { 
            new CategoryViewModel { Name = "Cat 1" } 
        };

        // Act
        var vm = new EditToDoItemViewModel(toDoItem, categories);

        // Assert
        Assert.Same(toDoItem, vm.Item);
        Assert.Equal(2, vm.AvailableCategories.Count);
        Assert.Same(CategoryViewModel.Empty, vm.AvailableCategories[0]);
        Assert.Equal("Cat 1", vm.AvailableCategories[1].Name);
    }

    [Fact]
    public void EditToDoItemViewModel_SetCategoryToEmpty_UpdatesItemCategory()
    {
        // Arrange
        var toDoItem = new ToDoItemViewModel(new ToDoItem { Title = "Test" });
        var categories = new List<CategoryViewModel>();
        var vm = new EditToDoItemViewModel(toDoItem, categories);
        vm.Item.Category = new CategoryViewModel { Name = "Some Cat" };

        // Act
        vm.SetCategoryToEmptyCommand.Execute(null);

        // Assert
        Assert.Same(CategoryViewModel.Empty, vm.Item.Category);
    }

    [Fact]
    public async Task SaveCommand_Success_ReturnsResultAndClosesDialog()
    {
        // Arrange
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Test Task" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoService { SaveResult = true };
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.NotNull(mockToDoService.SavedItem);
        Assert.Equal("Test Task", mockToDoService.SavedItem.Title);
        Assert.True(mockDialog.ReturnResultCalled);
        Assert.IsType<ToDoItemViewModel>(mockDialog.ReturnedResult);
        Assert.Equal("Test Task", ((ToDoItemViewModel)mockDialog.ReturnedResult!).Title);
    }

    [Fact]
    public async Task SaveCommand_ValidationError_ShowsErrorDialog()
    {
        // Arrange
        var item = new ToDoItemViewModel(new ToDoItem { Title = "Valid" });
        var categories = new List<CategoryViewModel>();
        var mockDialog = new MockDialogService();
        var mockToDoService = new MockToDoService();
        var vm = new EditToDoItemViewModel(item, categories, mockToDoService, mockDialog);

        // Trigger validation
        vm.Item.Title = null;
        vm.Item.Validate();

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.True(vm.Item.HasErrors);
        Assert.Equal("Error", mockDialog.LastTitle);
        Assert.False(mockDialog.ReturnResultCalled);
    }
}
