using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using Avalonia.Media;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Comprehensive unit tests for CategoryViewModel.
/// Tests all aspects of CategoryViewModel including initialization,
/// model conversion, equality, cloning, and property change notifications.
/// </summary>
/// <remarks>
/// Why test CategoryViewModel?
/// - Validates ViewModel behavior independently of the UI
/// - Ensures proper wrapping of the Category model
/// - Tests observable property change notifications
/// - Verifies validation and equality logic
/// - Confirms immutability patterns
/// 
/// Test categories covered:
/// - Constructor behavior: Default values and model initialization
/// - Color handling: Random colors, invalid colors, null colors
/// - Empty category: Special uncategorized category behavior
/// - Model conversion: ToCategory() method
/// - Equality: ID-based comparison, null handling
/// - Operators: ==, !=, GetHashCode()
/// - Cloning: Independent copies for editing
/// - Property notifications: PropertyChanged events
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Theory tests with InlineData for multiple scenarios
/// - Event subscription for notification testing
/// - Independent test isolation (no shared state)
/// </remarks>
public class CategoryViewModelTest
{
    /// <summary>
    /// Tests that the parameterless constructor sets correct default values.
    /// New categories should have a random color and default name.
    /// </summary>
    /// <remarks>
    /// Expected defaults:
    /// - ID: null (not yet saved to the database)
    /// - Name: "New Category" (placeholder for user editing)
    /// - Color: Random color (visual distinction in UI)
    /// - Description: null (optional field)
    /// </remarks>
    [Fact]
    public void CategoryViewModel_DefaultConstructor_SetsDefaults()
    {
        // Arrange & Act - Create new CategoryViewModel with default constructor
        var viewModel = new CategoryViewModel();
 
        // Assert - Verify default values are set correctly
        Assert.Null(viewModel.Id);
        Assert.Equal("New Category", viewModel.Name);
        
        // Color should be random, so just verify it's not the default color and the color is fully opaque
        Assert.NotEqual(default(Color), viewModel.Color);
        Assert.Equal(255, viewModel.Color.A);
    }
 
    /// <summary>
    /// Tests that ViewModel initializes correctly from a Category model.
    /// Verifies all model properties are correctly copied to ViewModel.
    /// </summary>
    /// <remarks>
    /// This test ensures the ViewModel properly wraps the data model.
    /// The ViewModel adds UI-specific features while preserving data.
    /// </remarks>
    [Fact]
    public void CategoryViewModel_FromModel_InitializesCorrectly()
    {
        // Arrange - Create a Category model with test data
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "Red"
        };
 
        // Act - Create ViewModel from model
        var viewModel = new CategoryViewModel(model);
 
        // Assert - Verify all properties are correctly initialized
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Work", viewModel.Name);
        Assert.Equal("Work tasks", viewModel.Description);
        
        // Color should be parsed from string to a Color object
        Assert.Equal(Color.Parse("#FF0000"), viewModel.Color);
    }
 
    /// <summary>
    /// Tests that ViewModel uses random color when the model color is invalid.
    /// Ensures graceful handling of malformed color strings.
    /// </summary>
    /// <remarks>
    /// Invalid color scenarios:
    /// - Non-existent color names
    /// - Malformed hex codes
    /// - Invalid CSS formats
    /// 
    /// Fallback behavior:
    /// - Use random color instead of failing
    /// - Prevents application crashes from bad data
    /// </remarks>
    [Fact]
    public void CategoryViewModel_FromModelWithInvalidColor_UsesRandomColor()
    {
        // Arrange - Create Category with invalid color string
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Color = "invalid-color"
        };
 
        // Act - Create ViewModel from model
        var viewModel = new CategoryViewModel(model);
 
        // Assert - Verify random color is used (not default)
        Assert.NotEqual(default(Color), viewModel.Color);
        Assert.Equal(255, viewModel.Color.A);
    }
 
    /// <summary>
    /// Tests that ViewModel uses random color when a model color is null.
    /// Ensures new categories always have a visible color.
    /// </summary>
    /// <remarks>
    /// Null color handling:
    /// - Common for newly created categories
    /// - Random color ensures visual distinction
    /// - Prevents null reference issues in UI
    /// </remarks>
    [Fact]
    public void CategoryViewModel_FromModelWithNullColor_UsesRandomColor()
    {
        // Arrange - Create Category with null color
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Color = null
        };
 
        // Act - Create ViewModel from model
        var viewModel = new CategoryViewModel(model);
 
        // Assert - Verify random color is used (not default) and the color is opaque
        Assert.NotEqual(default(Color), viewModel.Color);
        Assert.Equal(255, viewModel.Color.A);
    }
 
    /// <summary>
    /// Tests that the Empty static instance has correct default values.
    /// Empty category represents "uncategorized" items.
    /// </summary>
    /// <remarks>
    /// Empty category behavior:
    /// - Special singleton instance
    /// - Represents items without a category
    /// - Has distinctive gray color
    /// - Name: "Uncategorized"
    /// </remarks>
    [Fact]
    public void CategoryViewModel_Empty_HasCorrectValues()
    {
        // Arrange & Act - Get the static Empty instance
        var empty = CategoryViewModel.Empty;
 
        // Assert - Verify Empty category properties
        Assert.Null(empty.Id); // No database ID
        Assert.Equal("Uncategorized", empty.Name);
        Assert.Equal(Color.FromArgb(255, 150, 150, 150), empty.Color); // Gray color
    }
 
    /// <summary>
    /// Tests that ToCategory() converts ViewModel back to model correctly.
    /// Verifies round-trip conversion preserves all data.
    /// </summary>
    /// <remarks>
    /// Round-trip test:
    /// Model → ViewModel → Model
    /// Ensures no data loss during conversion
    /// </remarks>
    [Fact]
    public void CategoryViewModel_ToCategory_ConvertsCorrectly()
    {
        // Arrange - Create ViewModel from test model
        var originalModel = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "Red"
        };

        var viewModel = new CategoryViewModel(originalModel);
 
        // Act - Convert back to model
        var newModel = viewModel.ToCategory();
 
        // Assert - Verify all properties are preserved. 
        // Since our Model is a record, we can use record equality.
        Assert.Equal(originalModel, newModel);
    }
 
    /// <summary>
    /// Tests that equality compares by ID only.
    /// CategoryViewModel equality is based on ID, not other properties.
    /// </summary>
    /// <remarks>
    /// Equality rules:
    /// - Same ID → Equal (even if different names/colors)
    /// - Different ID → Not equal
    /// - Null handling: Comparing with null returns false
    /// 
    /// This is important for:
    /// - Lookup operations in collections
    /// - Dictionary key comparisons
    /// - Update detection in caches
    /// </remarks>
    [Fact]
    public void CategoryViewModel_Equals_ComparesById()
    {
        // Arrange - Create three ViewModels with different properties
        var category1 = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Work",
            Color = "#FF0000"
        });
 
        var category2 = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Different Name", // Different name, same ID
            Color = "#00FF00"
        });
 
        var category3 = new CategoryViewModel(new Category
        {
            Id = 2, // Different ID
            Name = "Work",
            Color = "#FF0000"
        });
 
        // Act & Assert
        // Same ID → Equal (despite different name/color)
        Assert.True(category1.Equals(category2));
        // Different ID → Not equal (despite the same name and color)
        Assert.False(category1.Equals(category3));
    }
 
    /// <summary>
    /// Tests that == operator works correctly for equality.
    /// Operator should behave the same as the Equals () method.
    /// </summary>
    [Fact]
    public void CategoryViewModel_EqualsOperator_WorksCorrectly()
    {
        // Arrange - Create three ViewModels
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category3 = new CategoryViewModel(new Category { Id = 2, Name = "Personal" });
 
        // Act & Assert
        Assert.True(category1 == category2);
        Assert.False(category1 == category3);
    }
 
    /// <summary>
    /// Tests that != operator works correctly for inequality.
    /// Should be the logical negation of == operator.
    /// </summary>
    [Fact]
    public void CategoryViewModel_NotEqualsOperator_WorksCorrectly()
    {
        // Arrange - Create two ViewModels with different IDs
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 2, Name = "Personal" });
 
        // Act & Assert
        Assert.True(category1 != category2);
    }
 
    /// <summary>
    /// Tests that GetHashCode is based on ID.
    /// HashCode consistency is required for equality to work correctly.
    /// </summary>
    /// <remarks>
    /// GetHashCode rules:
    /// - Must be consistent with Equals()
    /// - Same ID → Same hash code
    /// - Different ID → Different hash code
    /// - Important for Dictionary and HashSet operations
    /// </remarks>
    [Fact]
    public void CategoryViewModel_GetHashCode_BasedOnId()
    {
        // Arrange - Create three ViewModels
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 1, Name = "Different" });
        var category3 = new CategoryViewModel(new Category { Id = 2, Name = "Work" });
 
        // Act & Assert
        // Same ID → Same hash code (despite different name)
        Assert.Equal(category1.GetHashCode(), category2.GetHashCode());
        // Different ID → Different hash code
        Assert.NotEqual(category1.GetHashCode(), category3.GetHashCode());
    }
 
    /// <summary>
    /// Tests that Clone() creates an independent copy of the ViewModel.
    /// Cloning is important for creating edit dialogs without affecting the original.
    /// </summary>
    /// <remarks>
    /// Cloning behavior:
    /// - Creates shallow copy (new instance, same values)
    /// - Original remains unchanged (independent)
    /// - Changes to clone don't affect original
    /// - Used for edit dialog workflows
    /// </remarks>
    [Fact]
    public void CategoryViewModel_Clone_CreatesIndependentCopy()
    {
        // Arrange - Create original ViewModel
        var original = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Original",
            Color = "#FF0000"
        });
 
        // Act - Clone and modify the copy
        var cloned = (CategoryViewModel)original.Clone();
        cloned.Name = "Modified";
        cloned.Description = "New Description";
        cloned.Color = Colors.Blue;
 
        // Assert - Verify original is unchanged
        Assert.Equal("Original", original.Name);
        Assert.Null(original.Description);
        Assert.Equal(Colors.Red, original.Color);
        
        // Verify that the clone has new values
        Assert.Equal("Modified", cloned.Name);
        Assert.Equal("New Description", cloned.Description);
        Assert.Equal(Colors.Blue, cloned.Color);
    }
 
    /// <summary>
    /// Tests that property changes trigger PropertyChanged notifications.
    /// Validates observable property pattern implementation.
    /// </summary>
    /// <remarks>
    /// PropertyChanged events are critical for:
    /// - UI data binding updates
    /// - Collection change notifications
    /// - Reactive programming subscriptions
    /// 
    /// All observable properties should notify when changed.
    /// </remarks>
    [Fact]
    public void CategoryViewModel_PropertyChanges_NotifyCorrectly()
    {
        // Arrange - Create ViewModel and track property changes
        var viewModel = new CategoryViewModel();
        var nameChanged = false;
        var descriptionChanged = false;
        var colorChanged = false;
 
        // Subscribe to PropertyChanged event
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(CategoryViewModel.Name))
                nameChanged = true;
            if (e.PropertyName == nameof(CategoryViewModel.Description))
                descriptionChanged = true;
            if (e.PropertyName == nameof(CategoryViewModel.Color))
                colorChanged = true;
        };
 
        // Act - Change all properties
        viewModel.Name = "Updated";
        viewModel.Description = "New Description";
        viewModel.Color = Colors.Blue;
 
        // Assert - Verify all properties raised notifications
        Assert.True(nameChanged);
        Assert.True(descriptionChanged);
        Assert.True(colorChanged);
    }
 
    /// <summary>
    /// Tests that Equals returns false when comparing with null.
    /// Proper null handling prevents runtime exceptions.
    /// </summary>
    /// <remarks>
    /// Null comparison scenarios:
    /// - category.Equals(null) → false
    /// - category == null → false
    /// - category != null → true
    /// </remarks>
    [Fact]
    public void CategoryViewModel_EqualsWithNull_ReturnsFalse()
    {
        // Arrange - Create ViewModel
        var category = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
 
        // Act & Assert
        Assert.False(category.Equals(null));
        Assert.False(category == null);
        Assert.True(category != null);
    }
 
    /// <summary>
    /// Tests that Color property can be set to a new color.
    /// Verifies color assignment works for user-chosen colors.
    /// </summary>
    [Fact]
    public void CategoryViewModel_ColorProperty_CanBeSet()
    {
        // Arrange - Create ViewModel and test color
        var viewModel = new CategoryViewModel();
        var newColor = Color.Parse("#0000FF");
 
        // Act - Set color property
        viewModel.Color = newColor;
 
        // Assert - Verify color is stored correctly
        Assert.Equal(newColor, viewModel.Color);
    }
}
