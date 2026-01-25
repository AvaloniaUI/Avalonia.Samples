using AdvancedToDoList.Models;
using Xunit;

namespace AdvancedToDoListTests.ModelTests;

public class ToDoItemTests
{
    [Fact]
    public void ToDoItem_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var item = new ToDoItem();

        // Assert
        Assert.Null(item.Id);
        Assert.Null(item.Category);
        Assert.Null(item.CategoryId);
        Assert.Null(item.Title);
        Assert.Equal((int)Priority.Medium, item.Priority);
        Assert.Null(item.Description);
        Assert.InRange(item.DueDate, DateTime.Now.AddDays(6.9), DateTime.Now.AddDays(7.1));
        Assert.Equal(0, item.Progress);
        Assert.InRange(item.CreatedDate, DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
        Assert.Null(item.CompletedDate);
    }

    [Fact]
    public void ToDoItem_WithProperties_SetsCorrectly()
    {
        // Arrange
        var dueDate = DateTime.Now.AddDays(10);
        var createdDate = DateTime.Now;
        var category = new Category { Id = 1, Name = "Work" };

        // Act
        var item = new ToDoItem
        {
            Id = 1,
            CategoryId = 1,
            Category = category,
            Title = "Test Task",
            Priority = (int)Priority.High,
            Description = "Test Description",
            DueDate = dueDate,
            Progress = 50,
            CreatedDate = createdDate
        };

        // Assert
        Assert.Equal(1, item.Id);
        Assert.Equal(1, item.CategoryId);
        Assert.Equal(category, item.Category);
        Assert.Equal("Test Task", item.Title);
        Assert.Equal((int)Priority.High, item.Priority);
        Assert.Equal("Test Description", item.Description);
        Assert.Equal(dueDate, item.DueDate);
        Assert.Equal(50, item.Progress);
        Assert.Equal(createdDate, item.CreatedDate);
        Assert.Null(item.CompletedDate);
    }

    [Fact]
    public void ToDoItem_RecordEquality_WorksCorrectly()
    {
        // Arrange
        var item1 = new ToDoItem
        {
            Id = 1,
            Title = "Test",
            Priority = (int)Priority.Medium,
            DueDate = new DateTime(2025, 1, 1),
            Progress = 0,
            CreatedDate = new DateTime(2024, 1, 1)
        };

        var item2 = new ToDoItem
        {
            Id = 1,
            Title = "Test",
            Priority = (int)Priority.Medium,
            DueDate = new DateTime(2025, 1, 1),
            Progress = 0,
            CreatedDate = new DateTime(2024, 1, 1)
        };

        var item3 = new ToDoItem
        {
            Id = 2,
            Title = "Different",
            Priority = (int)Priority.High,
            DueDate = new DateTime(2025, 2, 1),
            Progress = 50,
            CreatedDate = new DateTime(2024, 2, 1)
        };

        // Act & Assert
        Assert.Equal(item1, item2);
        Assert.NotEqual(item1, item3);
    }

    [Fact]
    public void ToDoItem_With_CreatesModifiedCopy()
    {
        // Arrange
        var original = new ToDoItem
        {
            Id = 1,
            Title = "Original",
            Priority = (int)Priority.Low
        };

        // Act
        var modified = original with { Title = "Modified", Priority = (int)Priority.High };

        // Assert
        Assert.Equal(1, modified.Id);
        Assert.Equal("Modified", modified.Title);
        Assert.Equal((int)Priority.High, modified.Priority);
        Assert.Equal("Original", original.Title); // Original unchanged
    }

    [Theory]
    [InlineData(Priority.Low, (int)Priority.Low)]
    [InlineData(Priority.Medium, (int)Priority.Medium)]
    [InlineData(Priority.High, (int)Priority.High)]
    public void ToDoItem_Priority_CanBeSetToAllValues(Priority priority, int expectedValue)
    {
        // Arrange
        var item = new ToDoItem();

        // Act
        item.Priority = (int)priority;

        // Assert
        Assert.Equal(expectedValue, item.Priority);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void ToDoItem_Progress_CanBeSetToValidValues(int progress)
    {
        // Arrange
        var item = new ToDoItem();

        // Act
        item.Progress = progress;

        // Assert
        Assert.Equal(progress, item.Progress);
    }

    [Fact]
    public void ToDoItem_CompletedDate_CanBeSet()
    {
        // Arrange
        var item = new ToDoItem();
        var completedDate = DateTime.Now;

        // Act
        item.CompletedDate = completedDate;

        // Assert
        Assert.Equal(completedDate, item.CompletedDate);
    }
}