using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class ToDoItemViewModelTest
{
    [Fact]
    public void ToDoItemViewModel_Constructor_InitializesFromModel()
    {
        // Arrange
        var dueDate = DateTime.Now.AddDays(5);
        var createdDate = DateTime.Now.AddDays(-1);
        var category = new Category { Id = 1, Name = "Work", Color = "#FF0000" };
        var model = new ToDoItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            Priority = (int)Priority.High,
            DueDate = dueDate,
            Progress = 50,
            CreatedDate = createdDate,
            Category = category,
            CompletedDate = null
        };

        // Act
        var viewModel = new ToDoItemViewModel(model);

        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Test Task", viewModel.Title);
        Assert.Equal("Test Description", viewModel.Description);
        Assert.Equal(Priority.High, viewModel.Priority);
        Assert.Equal(dueDate, viewModel.DueDate);
        Assert.Equal(50, viewModel.Progress);
        Assert.Equal(createdDate, viewModel.CreatedDate);
        Assert.NotNull(viewModel.Category);
        Assert.Equal(1, viewModel.Category.Id);
        Assert.Null(viewModel.CompletedDate);
    }

    [Fact]
    public void ToDoItemViewModel_WithNullCategory_UsesEmptyCategory()
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Category = null
        };

        // Act
        var viewModel = new ToDoItemViewModel(model);

        // Assert
        Assert.NotNull(viewModel.Category);
        Assert.Equal(CategoryViewModel.Empty.Name, viewModel.Category.Name);
    }

    [Theory]
    [InlineData(0, ToDoItemStatus.NotStarted)]
    [InlineData(50, ToDoItemStatus.InProgress)]
    [InlineData(100, ToDoItemStatus.Done)]
    public void ToDoItemViewModel_CurrentStatus_BasedOnProgress(int progress, ToDoItemStatus expectedStatus)
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Progress = progress,
            DueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var viewModel = new ToDoItemViewModel(model);

        // Assert
        Assert.Equal(expectedStatus, viewModel.CurrentStatus);
    }

    [Fact]
    public void ToDoItemViewModel_CurrentStatus_Overdue_WhenDueDatePassed()
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Progress = 50,
            DueDate = DateTime.Now.AddDays(-1)
        };

        // Act
        var viewModel = new ToDoItemViewModel(model);

        // Assert
        Assert.Equal(ToDoItemStatus.Overdue, viewModel.CurrentStatus);
    }

    [Fact]
    public void ToDoItemViewModel_CurrentStatus_Done_OverridesOverdue()
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Progress = 100,
            DueDate = DateTime.Now.AddDays(-1)
        };

        // Act
        var viewModel = new ToDoItemViewModel(model);

        // Assert
        Assert.Equal(ToDoItemStatus.Done, viewModel.CurrentStatus);
    }

    [Fact]
    public void ToDoItemViewModel_ToToDoItem_ConvertsBackToModel()
    {
        // Arrange
        var dueDate = DateTime.Now.AddDays(5);
        var createdDate = DateTime.Now.AddDays(-1);
        var category = new Category { Id = 1, Name = "Work", Color = "#FF0000" };
        var model = new ToDoItem
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            Priority = (int)Priority.High,
            DueDate = dueDate,
            Progress = 50,
            CreatedDate = createdDate,
            Category = category
        };
        var viewModel = new ToDoItemViewModel(model);

        // Act
        var convertedModel = viewModel.ToToDoItem();

        // Assert
        Assert.Equal(1, convertedModel.Id);
        Assert.Equal("Test Task", convertedModel.Title);
        Assert.Equal("Test Description", convertedModel.Description);
        Assert.Equal((int)Priority.High, convertedModel.Priority);
        Assert.Equal(dueDate, convertedModel.DueDate);
        Assert.Equal(50, convertedModel.Progress);
        Assert.Equal(createdDate, convertedModel.CreatedDate);
        Assert.Equal(1, convertedModel.CategoryId);
    }

    [Fact]
    public void ToDoItemViewModel_Clone_CreatesIndependentCopy()
    {
        // Arrange
        var model = new ToDoItem
        {
            Id = 1,
            Title = "Original",
            Progress = 50
        };
        var viewModel = new ToDoItemViewModel(model);

        // Act
        var cloned = viewModel.CloneToDoItemViewModel();
        cloned.Title = "Modified";
        cloned.Progress = 75;

        // Assert
        Assert.Equal("Original", viewModel.Title);
        Assert.Equal(50, viewModel.Progress);
        Assert.Equal(CategoryViewModel.Empty.Name, viewModel.Category.Name);
        Assert.Equal("Modified", cloned.Title);
        Assert.Equal(75, cloned.Progress);
    }

    [Fact]
    public void ToDoItemViewModel_PropertyChanges_NotifyCorrectly()
    {
        // Arrange
        var model = new ToDoItem { Title = "Test" };
        var viewModel = new ToDoItemViewModel(model);
        var titleChanged = false;
        var progressChanged = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(ToDoItemViewModel.Title))
                titleChanged = true;
            if (e.PropertyName == nameof(ToDoItemViewModel.Progress))
                progressChanged = true;
        };

        // Act
        viewModel.Title = "Updated";
        viewModel.Progress = 75;

        // Assert
        Assert.True(titleChanged);
        Assert.True(progressChanged);
    }

    [Fact]
    public void ToDoItemViewModel_ProgressChange_UpdatesCurrentStatus()
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Progress = 0,
            DueDate = DateTime.Now.AddDays(7)
        };
        var viewModel = new ToDoItemViewModel(model);
        var statusChanged = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(ToDoItemViewModel.CurrentStatus))
                statusChanged = true;
        };

        // Act
        viewModel.Progress = 50;

        // Assert
        Assert.True(statusChanged);
        Assert.Equal(ToDoItemStatus.InProgress, viewModel.CurrentStatus);
    }

    [Fact]
    public void ToDoItemViewModel_ProgressSetTo100_SetsCompletedDate()
    {
        // Arrange
        var model = new ToDoItem { Title = "Test", Progress = 0 };
        var viewModel = new ToDoItemViewModel(model);

        // Act
        viewModel.Progress = 100;

        // Assert
        Assert.NotNull(viewModel.CompletedDate);
        Assert.InRange(viewModel.CompletedDate.Value, DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public void ToDoItemViewModel_ProgressReducedFrom100_ClearsCompletedDate()
    {
        // Arrange
        var model = new ToDoItem { Title = "Test", Progress = 100, CompletedDate = DateTime.Now };
        var viewModel = new ToDoItemViewModel(model);
        Assert.NotNull(viewModel.CompletedDate); // Verify initial state

        // Act
        viewModel.Progress = 50;

        // Assert
        Assert.Null(viewModel.CompletedDate);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    public void ToDoItemViewModel_Priority_CanBeSetToAllValues(Priority priority)
    {
        // Arrange
        var model = new ToDoItem { Title = "Test" };
        var viewModel = new ToDoItemViewModel(model);

        // Act
        viewModel.Priority = priority;

        // Assert
        Assert.Equal(priority, viewModel.Priority);
    }

    [Fact]
    public void ToDoItemViewModel_DueDateChange_UpdatesCurrentStatus()
    {
        // Arrange
        var model = new ToDoItem
        {
            Title = "Test",
            Progress = 50,
            DueDate = DateTime.Now.AddDays(7)
        };
        var viewModel = new ToDoItemViewModel(model);
        Assert.Equal(ToDoItemStatus.InProgress, viewModel.CurrentStatus);

        // Act
        viewModel.DueDate = DateTime.Now.AddDays(-1);

        // Assert
        Assert.Equal(ToDoItemStatus.Overdue, viewModel.CurrentStatus);
    }
}