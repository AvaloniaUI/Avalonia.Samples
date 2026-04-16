using AdvancedToDoList.Models;
using Xunit;

namespace AdvancedToDoListTests.ModelTests;

/// <summary>
/// Unit tests for the ToDoItem data model.
/// Tests all properties, default values, equality, and validation behaviors.
/// Ensures ToDoItem model works correctly as a foundational data structure.
/// </summary>
/// <remarks>
/// Why test ToDoItem model?
/// - Validates that data model behaves as expected
/// - Ensures record type equality works correctly
/// - Tests default values for new instances
/// - Verifies immutability patterns with 'with' operator
/// 
/// Test categories:
/// - Constructor behavior: Default values and initialization
/// - Property setting: Correct assignment and type handling
/// - Equality: Record-based equality for same/different instances
/// - Immutability: 'with' operator creates modified copies
/// - Priority values: Support for all enum values
/// - Progress values: Support for valid range (0-100)
/// - Date handling: Due date, created date, completed date
/// </remarks>
public class ToDoItemTests
{
    /// <summary>
    /// Tests that parameterless constructor initializes all properties correctly.
    /// Verifies default values match expected business rules.
    /// </summary>
    /// <remarks>
    /// Expected default values:
    /// - ID: null (not yet saved to database)
    /// - Category: null (no category assigned)
    /// - CategoryId: null (not yet saved to database)
    /// - Title: null (required field, will be validated later)
    /// - Priority: Medium (default priority level)
    /// - Description: null (optional field)
    /// - DueDate: 7 days from now (default due period)
    /// - Progress: 0 (not started)
    /// - CreatedDate: DateTime.Now (time of creation)
    /// - CompletedDate: null (not yet completed)
    /// </remarks>
    [Fact]
    public void ToDoItem_Constructor_SetsDefaultValues()
    {
        // Arrange & Act - Create new ToDoItem using default constructor
        var item = new ToDoItem();
 
        // Assert - Verify all default values are correct
        Assert.Null(item.Id);
        Assert.Null(item.Category);
        Assert.Null(item.CategoryId);
        Assert.Null(item.Title);
        Assert.Equal((int)Priority.Medium, item.Priority);
        Assert.Null(item.Description);
        
        // DueDate should be approximately 7 days from now
        // Use InRange because DateTime.Now changes between lines
        Assert.InRange(item.DueDate, DateTime.Now.AddDays(6.9), DateTime.Now.AddDays(7.1));
        
        Assert.Equal(0, item.Progress);
        
        // CreatedDate should be approximately now
        // Use InRange because DateTime.Now changes between lines
        Assert.InRange(item.CreatedDate, DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
        
        Assert.Null(item.CompletedDate);
    }
 
    /// <summary>
    /// Tests that all properties can be set via object initializer.
    /// Verifies property assignment works correctly for initialization.
    /// </summary>
    /// <remarks>
    /// This test ensures that when creating a ToDoItem with values,
    /// all properties are correctly assigned and can be retrieved.
    /// </remarks>
    [Fact]
    public void ToDoItem_WithProperties_SetsCorrectly()
    {
        // Arrange - Prepare test data
        var dueDate = DateTime.Now.AddDays(10);
        var createdDate = DateTime.Now;
        var category = new Category { Id = 1, Name = "Work" };
 
        // Act - Create ToDoItem with all properties set
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
 
        // Assert - Verify all properties are set correctly
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
 
    /// <summary>
    /// Tests that record-based equality works correctly.
    /// Records automatically compare all properties for equality.
    /// </summary>
    /// <remarks>
    /// Expected equality behavior:
    /// - Same ID, Title, Priority, DueDate, Progress, CreatedDate → Equal
    /// - Different property values → Not equal
    /// - Record equality is value-based, not reference-based
    /// </remarks>
    [Fact]
    public void ToDoItem_RecordEquality_WorksCorrectly()
    {
        // Arrange - Create three ToDoItems with different values
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
        // item1 and item2 have identical values → should be equal
        Assert.Equal(item1, item2);
        // item1 and item3 have different values → should not be equal
        Assert.NotEqual(item1, item3);
    }
 
    /// <summary>
    /// Tests that 'with' operator creates modified copies.
    /// Records support non-destructive modification by creating new instances.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Original remains unchanged (immutability)
    /// - New instance has specified changes
    /// - Unspecified properties are copied from the original
    /// </remarks>
    [Fact]
    public void ToDoItem_With_CreatesModifiedCopy()
    {
        // Arrange - Create original ToDoItem
        var original = new ToDoItem
        {
            Id = 1,
            Title = "Original",
            Priority = (int)Priority.Low
        };
 
        // Act - Create modified copy using 'with' operator
        var modified = original with { Title = "Modified", Priority = (int)Priority.High };
 
        // Assert - Verify the new instance has changes
        Assert.Equal(1, modified.Id);
        Assert.Equal("Modified", modified.Title);
        Assert.Equal((int)Priority.High, modified.Priority);
        
        // Verify the original is unchanged (immutability)
        Assert.Equal("Original", original.Title);
    }
 
    /// <summary>
    /// Tests that priority can be set to all possible enum values.
    /// Validates that all three priority levels work correctly.
    /// </summary>
    /// <param name="priority">The priority enum value to test</param>
    /// <param name="expectedValue">The expected integer representation</param>
    [Theory]
    [InlineData(Priority.Low, (int)Priority.Low)]
    [InlineData(Priority.Medium, (int)Priority.Medium)]
    [InlineData(Priority.High, (int)Priority.High)]
    public void ToDoItem_Priority_CanBeSetToAllValues(Priority priority, int expectedValue)
    {
        // Arrange
        var item = new ToDoItem();
 
        // Act - Set priority to the specified value
        item.Priority = (int)priority;
 
        // Assert - Verify priority is stored correctly
        Assert.Equal(expectedValue, item.Priority);
    }
 
    /// <summary>
    /// Tests that progress can be set to all valid values.
    /// Validates that the progress accepts the full 0-100 range.
    /// </summary>
    /// <param name="progress">The progress value to test</param>
    /// <remarks>
    /// Valid progress range is 0-100:
    /// - 0: Not started
    /// - 25: Quarter complete
    /// - 50: Half complete
    /// - 75: Three-quarters complete
    /// - 100: Fully complete
    /// </remarks>
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
 
        // Act - Set progress to the specified value
        item.Progress = progress;
 
        // Assert - Verify progress is stored correctly
        Assert.Equal(progress, item.Progress);
    }
 
    /// <summary>
    /// Tests that completed date can be set when the item is done.
    /// Validates completion timestamp functionality.
    /// </summary>
    /// <remarks>
    /// Completed date behavior:
    /// - Can be set to any DateTime
    /// - Usually set to DateTime.Now when progress reaches 100
    /// - null indicates that the item is not yet completed
    /// </remarks>
    [Fact]
    public void ToDoItem_CompletedDate_CanBeSet()
    {
        // Arrange
        var item = new ToDoItem();
        var completedDate = DateTime.Now;
 
        // Act - Set completed date
        item.CompletedDate = completedDate;
 
        // Assert - Verify completed date is stored correctly
        Assert.Equal(completedDate, item.CompletedDate);
    }
}
