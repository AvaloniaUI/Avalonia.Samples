using AdvancedToDoList.Models;
using Avalonia.Media;
using Xunit;

namespace AdvancedToDoListTests.ModelTests;

/// <summary>
/// Unit tests for the Category data model.
/// Tests all properties, default values, equality, and validation behaviors.
/// Ensures the Category model works correctly as a foundational data structure.
/// </summary>
/// <remarks>
/// Why test the Category model?
/// - Validates that the data model behaves as expected
/// - Ensures record type equality works correctly
/// - Tests default values for new instances
/// - Verifies immutability patterns with 'with' operator
/// 
/// Test categories:
/// - Constructor behavior: Default values and initialization
/// - Property setting: Correct assignment and type handling
/// - Equality: Record-based equality for same/different instances
/// - Immutability: 'with' operator creates modified copies
/// - Color formats: Support for different color representations
/// - Nullability: All properties can be null
/// </remarks>
public class CategoryTests
{
    /// <summary>
    /// Tests that parameterless constructor initializes all properties to null.
    /// Ensures new Category instances start in a clean, uninitialized state.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - ID should be null (not yet saved to a database)
    /// - Name should be null (required property, will be validated later)
    /// - Description should be null (optional field)
    /// - Color should be null (will be assigned by ViewModel)
    /// </remarks>
    [Fact]
    public void Category_Constructor_SetsDefaultValues()
    {
        // Arrange & Act - Create new Category using default constructor
        var category = new Category();
 
        // Assert - Verify all properties are null (uninitialized)
        Assert.Null(category.Id);
        Assert.Null(category.Name);
        Assert.Null(category.Description);
        Assert.Null(category.Color);
    }
 
    /// <summary>
    /// Tests that all properties can be set via object initializer.
    /// Verifies property assignment works correctly for initialization.
    /// </summary>
    /// <remarks>
    /// This test ensures that when creating a Category with values,
    /// all properties are correctly assigned and can be retrieved.
    /// </remarks>
    [Fact]
    public void Category_WithProperties_SetsCorrectly()
    {
        // Arrange & Act - Create Category with all properties set
        var category = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work related tasks",
            Color = "#FF0000"
        };
 
        // Assert - Verify all properties are set correctly
        Assert.Equal(1, category.Id);
        Assert.Equal("Work", category.Name);
        Assert.Equal("Work related tasks", category.Description);
        Assert.Equal("#FF0000", category.Color);
    }
 
    /// <summary>
    /// Tests that record-based equality works as expected.
    /// Records automatically compare all properties for equality.
    /// </summary>
    /// <remarks>
    /// Expected equality behavior:
    /// - Same ID, Name, Description, Color → Equal
    /// - Different property values → Not equal
    /// - Record equality is value-based, not reference-based
    /// </remarks>
    [Fact]
    public void Category_RecordEquality_WorksCorrectly()
    {
        // Arrange - Create three categories with different values
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
        // category1 and category2 have identical values → should be equal
        Assert.Equal(category1, category2);
        // category1 and category3 have different values → should not be equal
        Assert.NotEqual(category1, category3);
    }
 
    /// <summary>
    /// Tests that the 'with' operator creates modified copies.
    /// Records support non-destructive modification by creating new instances.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Original remains unchanged (immutability)
    /// - New instance has specified changes
    /// - Unspecified properties are copied from the original
    /// </remarks>
    [Fact]
    public void Category_With_CreatesModifiedCopy()
    {
        // Arrange - Create original Category
        var original = new Category
        {
            Id = 1,
            Name = "Original",
            Description = "Original Description",
            Color = "#FF0000"
        };
 
        // Act - Create modified copy using 'with' operator
        var modified = original with { Name = "Modified", Color = "#00FF00" };
 
        // Assert - Verify the new instance has changes
        Assert.Equal(1, modified.Id);
        Assert.Equal("Modified", modified.Name);
        Assert.Equal("Original Description", modified.Description);
        Assert.Equal("#00FF00", modified.Color);
        // Verify the original is unchanged (immutability)
        Assert.Equal("Original", original.Name);
    }
 
    
    /// <summary>
    /// Tests that Color property accepts different format representations.
    /// Supports hex codes, CSS rgb() format, and color names.
    /// </summary>
    /// <param name="color">The color string in various formats</param>
    /// <param name="parsedColorHex">The hexadecimal representation of the parsed Avalonia.Media.Color</param>
    /// <remarks>
    /// Color formats tested:
    /// - Hex: #FF0000 (red), #00FF00 (green), #330000FF (semi-transparent blue)
    /// - CSS rgb(): rgb(255, 0, 0) format
    /// - Named colors: #FFFFFF (white)
    ///
    /// If we convert it to Avalonia.Media.Color and call <see cref="Color.ToUInt32"/>,
    /// we can verify that the color was parsed correctly.
    /// Since we cannot pass a color directly, we have to compare the integer representation for comparison.
    /// </remarks>
    [Theory]
    [InlineData("#FF0000", 0xFFFF0000)]
    [InlineData("#00FF00", 0xFF00FF00)]
    [InlineData("#330000FF", 0x330000FF)]
    [InlineData("White", 0xFFFFFFFF)]
    [InlineData("rgb(255, 0, 0)", 0xFFFF0000)]
    public void Category_Color_CanBeSetToDifferentFormats(string color, UInt32 parsedColorHex)
    {
        // Arrange
        var category = new Category();
 
        // Act - Set color property
        category.Color = color;
        
        // Assert - Verify color is stored correctly
        Assert.Equal(color, category.Color);
        
        // Assert - Verify the color could be parsed
        Assert.Equal(parsedColorHex, Color.Parse(category.Color).ToUInt32());
    }
 
    /// <summary>
    /// Tests that all properties can be set to null.
    /// Validates nullable property behavior for the Category model.
    /// </summary>
    /// <remarks>
    /// All properties should accept null:
    /// - ID: null for unsaved categories
    /// - Name: null (though validated as required by ViewModel)
    /// - Description: null for optional description
    /// - Color: null for unassigned color
    /// </remarks>
    [Fact]
    public void Category_AllProperties_CanBeNull()
    {
        // Arrange & Act - Create Category with all null properties
        var category = new Category
        {
            Id = null,
            Name = null,
            Description = null,
            Color = null
        };
 
        // Assert - Verify all properties are null
        Assert.Null(category.Id);
        Assert.Null(category.Name);
        Assert.Null(category.Description);
        Assert.Null(category.Color);
    }
}
