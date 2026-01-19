using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using AdvancedToDoList.ViewModels;
using SharedControls.Controls;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class EditCategoryViewModelTest
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

    private class MockCategoryService : ICategoryService
    {
        public bool SaveResult { get; set; } = true;
        public Category? SavedCategory { get; private set; }

        public Task<bool> SaveCategoryAsync(Category category)
        {
            SavedCategory = category;
            return Task.FromResult(SaveResult);
        }

        public Task<bool> DeleteCategoryAsync(Category category)
        {
            return Task.FromResult(true);
        }
    }

    [Fact]
    public void EditCategoryViewModel_DefaultConstructor_CreatesNewItem()
    {
        // Act
        var vm = new EditCategoryViewModel();

        // Assert
        Assert.NotNull(vm.Item);
        Assert.Null(vm.Item.Id);
    }

    [Fact]
    public void EditCategoryViewModel_ConstructorWithItem_UsesProvidedItem()
    {
        // Arrange
        var category = new CategoryViewModel();
        
        // Act
        var vm = new EditCategoryViewModel(category);

        // Assert
        Assert.Same(category, vm.Item);
    }

    [Fact]
    public async Task SaveCommand_Success_ReturnsResultAndClosesDialog()
    {
        // Arrange
        var category = new CategoryViewModel { Name = "Test Category" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService { SaveResult = true };
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.True(mockCategoryService.SavedCategory != null);
        Assert.Equal("Test Category", mockCategoryService.SavedCategory.Name);
        Assert.True(mockDialog.ReturnResultCalled);
        Assert.IsType<CategoryViewModel>(mockDialog.ReturnedResult);
        Assert.Equal("Test Category", ((CategoryViewModel)mockDialog.ReturnedResult!).Name);
    }

    [Fact]
    public async Task SaveCommand_ValidationError_ShowsErrorDialog()
    {
        // Arrange
        var category = new CategoryViewModel { Name = "Valid" };
        var mockDialog = new MockDialogService();
        var mockCategoryService = new MockCategoryService();
        var vm = new EditCategoryViewModel(category, mockCategoryService, mockDialog);
        
        // Trigger validation by setting to null
        vm.Item.Name = null; 
        vm.Item.Validate();

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.True(vm.Item.HasErrors);
        Assert.Equal("Error", mockDialog.LastTitle);
        Assert.Equal("Please correct the errors in the form.", mockDialog.LastContent);
        Assert.False(mockDialog.ReturnResultCalled);
    }
}
