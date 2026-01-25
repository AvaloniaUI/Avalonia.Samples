using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using Avalonia.Media;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

public class CategoryViewModelTest
{
    [Fact]
    public void CategoryViewModel_DefaultConstructor_SetsDefaults()
    {
        // Arrange & Act
        var viewModel = new CategoryViewModel();

        // Assert
        Assert.Null(viewModel.Id);
        Assert.Equal("New Category", viewModel.Name);
        Assert.NotEqual(default(Color), viewModel.Color);
    }

    [Fact]
    public void CategoryViewModel_FromModel_InitializesCorrectly()
    {
        // Arrange
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "Red"
        };

        // Act
        var viewModel = new CategoryViewModel(model);

        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Work", viewModel.Name);
        Assert.Equal("Work tasks", viewModel.Description);
        Assert.Equal(Color.Parse("#FF0000"), viewModel.Color);
    }

    [Fact]
    public void CategoryViewModel_FromModelWithInvalidColor_UsesRandomColor()
    {
        // Arrange
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Color = "invalid-color"
        };

        // Act
        var viewModel = new CategoryViewModel(model);

        // Assert
        Assert.NotEqual(default(Color), viewModel.Color);
    }

    [Fact]
    public void CategoryViewModel_FromModelWithNullColor_UsesRandomColor()
    {
        // Arrange
        var model = new Category
        {
            Id = 1,
            Name = "Work",
            Color = null
        };

        // Act
        var viewModel = new CategoryViewModel(model);

        // Assert
        Assert.NotEqual(default(Color), viewModel.Color);
    }

    [Fact]
    public void CategoryViewModel_Empty_HasCorrectValues()
    {
        // Arrange & Act
        var empty = CategoryViewModel.Empty;

        // Assert
        Assert.Null(empty.Id);
        Assert.Equal("Uncategorized", empty.Name);
        Assert.Equal(Color.FromArgb(255, 150, 150, 150), empty.Color);
    }

    [Fact]
    public void CategoryViewModel_ToCategory_ConvertsCorrectly()
    {
        // Arrange
        var viewModel = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Work",
            Description = "Work tasks",
            Color = "#FF0000"
        });

        // Act
        var model = viewModel.ToCategory();

        // Assert
        Assert.Equal(1, model.Id);
        Assert.Equal("Work", model.Name);
        Assert.Equal("Work tasks", model.Description);
        Assert.NotNull(model.Color);
    }

    [Fact]
    public void CategoryViewModel_Equals_ComparesById()
    {
        // Arrange
        var category1 = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Work",
            Color = "#FF0000"
        });

        var category2 = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Different Name",
            Color = "#00FF00"
        });

        var category3 = new CategoryViewModel(new Category
        {
            Id = 2,
            Name = "Work",
            Color = "#FF0000"
        });

        // Act & Assert
        Assert.True(category1.Equals(category2));
        Assert.False(category1.Equals(category3));
    }

    [Fact]
    public void CategoryViewModel_EqualsOperator_WorksCorrectly()
    {
        // Arrange
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category3 = new CategoryViewModel(new Category { Id = 2, Name = "Personal" });

        // Act & Assert
        Assert.True(category1 == category2);
        Assert.False(category1 == category3);
    }

    [Fact]
    public void CategoryViewModel_NotEqualsOperator_WorksCorrectly()
    {
        // Arrange
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 2, Name = "Personal" });

        // Act & Assert
        Assert.True(category1 != category2);
    }

    [Fact]
    public void CategoryViewModel_GetHashCode_BasedOnId()
    {
        // Arrange
        var category1 = new CategoryViewModel(new Category { Id = 1, Name = "Work" });
        var category2 = new CategoryViewModel(new Category { Id = 1, Name = "Different" });
        var category3 = new CategoryViewModel(new Category { Id = 2, Name = "Work" });

        // Act & Assert
        Assert.Equal(category1.GetHashCode(), category2.GetHashCode());
        Assert.NotEqual(category1.GetHashCode(), category3.GetHashCode());
    }

    [Fact]
    public void CategoryViewModel_Clone_CreatesIndependentCopy()
    {
        // Arrange
        var original = new CategoryViewModel(new Category
        {
            Id = 1,
            Name = "Original",
            Color = "#FF0000"
        });

        // Act
        var cloned = (CategoryViewModel)original.Clone();
        cloned.Name = "Modified";
        cloned.Description = "New Description";
        cloned.Color = Colors.Blue;

        // Assert
        Assert.Equal("Original", original.Name);
        Assert.Null(original.Description);
        Assert.Equal(Colors.Red, original.Color);
        
        Assert.Equal("Modified", cloned.Name);
        Assert.Equal("New Description", cloned.Description);
        Assert.Equal(Colors.Blue, cloned.Color);
    }

    [Fact]
    public void CategoryViewModel_PropertyChanges_NotifyCorrectly()
    {
        // Arrange
        var viewModel = new CategoryViewModel();
        var nameChanged = false;
        var descriptionChanged = false;
        var colorChanged = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(CategoryViewModel.Name))
                nameChanged = true;
            if (e.PropertyName == nameof(CategoryViewModel.Description))
                descriptionChanged = true;
            if (e.PropertyName == nameof(CategoryViewModel.Color))
                colorChanged = true;
        };

        // Act
        viewModel.Name = "Updated";
        viewModel.Description = "New Description";
        viewModel.Color = Colors.Blue;

        // Assert
        Assert.True(nameChanged);
        Assert.True(descriptionChanged);
        Assert.True(colorChanged);
    }

    [Fact]
    public void CategoryViewModel_EqualsWithNull_ReturnsFalse()
    {
        // Arrange
        var category = new CategoryViewModel(new Category { Id = 1, Name = "Work" });

        // Act & Assert
        Assert.False(category.Equals(null));
        Assert.False(category == null);
        Assert.True(category != null);
    }

    [Fact]
    public void CategoryViewModel_ColorProperty_CanBeSet()
    {
        // Arrange
        var viewModel = new CategoryViewModel();
        var newColor = Color.Parse("#0000FF");

        // Act
        viewModel.Color = newColor;

        // Assert
        Assert.Equal(newColor, viewModel.Color);
    }
}
