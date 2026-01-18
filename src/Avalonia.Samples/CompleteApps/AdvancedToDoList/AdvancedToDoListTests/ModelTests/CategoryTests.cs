using AdvancedToDoList.Models;
using Xunit;

namespace AdvancedToDoListTests.ModelTests;

public class CategoryTests
{
    [Fact]
    public void Category_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        Assert.Null(category.Id);
        Assert.Null(category.Name);
        Assert.Null(category.Description);
        Assert.Null(category.Color);
    }

    [Fact]
    public void Category_WithProperties_SetsCorrectly()
    {
        // Arrange & Act
        var category = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work related tasks",
            Color = "#FF0000"
        };

        // Assert
        Assert.Equal(1, category.Id);
        Assert.Equal("Work", category.Name);
        Assert.Equal("Work related tasks", category.Description);
        Assert.Equal("#FF0000", category.Color);
    }

    [Fact]
    public void Category_RecordEquality_WorksCorrectly()
    {
        // Arrange
        var category1 = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "#FF0000"
        };

        var category2 = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "#FF0000"
        };

        var category3 = new Category
        {
            Id = 2,
            Name = "Personal",
            Description = "Personal tasks",
            Color = "#00FF00"
        };

        // Act & Assert
        Assert.Equal(category1, category2);
        Assert.NotEqual(category1, category3);
    }

    [Fact]
    public void Category_With_CreatesModifiedCopy()
    {
        // Arrange
        var original = new Category
        {
            Id = 1,
            Name = "Original",
            Description = "Original Description",
            Color = "#FF0000"
        };

        // Act
        var modified = original with { Name = "Modified", Color = "#00FF00" };

        // Assert
        Assert.Equal(1, modified.Id);
        Assert.Equal("Modified", modified.Name);
        Assert.Equal("Original Description", modified.Description);
        Assert.Equal("#00FF00", modified.Color);
        Assert.Equal("Original", original.Name); // Original unchanged
    }

    [Theory]
    [InlineData("#FF0000")]
    [InlineData("#00FF00")]
    [InlineData("#0000FF")]
    [InlineData("#FFFFFF")]
    [InlineData("rgb(255, 0, 0)")]
    public void Category_Color_CanBeSetToDifferentFormats(string color)
    {
        // Arrange
        var category = new Category();

        // Act
        category.Color = color;

        // Assert
        Assert.Equal(color, category.Color);
    }

    [Fact]
    public void Category_AllProperties_CanBeNull()
    {
        // Arrange & Act
        var category = new Category
        {
            Id = null,
            Name = null,
            Description = null,
            Color = null
        };

        // Assert
        Assert.Null(category.Id);
        Assert.Null(category.Name);
        Assert.Null(category.Description);
        Assert.Null(category.Color);
    }
}