using AdvancedToDoList.Models;
using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for ToDoItemViewModel.
/// Comprehensive test suite covering construction, state management, 
/// status calculation, conversion, cloning, property change notifications, 
/// and edge cases for progress/due date interactions.
/// </summary>
/// <remarks>
/// Why test ToDoItemViewModel?
/// - Core ViewModel for to-do items - must handle all state transitions correctly
/// - Encapsulates business logic (status calculation, overdue detection)
/// - Serves as data model for UI components and dialogs
/// - Converts between database models (ToDoItem) and UI-facing ViewModels
/// 
/// Test categories covered:
/// - Construction: Initialization from model, null category handling
/// - Status logic: CurrentStatus calculation based on progress and due date
/// - Conversion: Two-way conversion between ViewModel and database models
/// - Cloning: Independent copy creation for safe editing
/// - Property changes: INotifyPropertyChanged implementation
/// - Edge cases: Completed date management, priority all-values coverage
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Theory/InlineData for parameterized testing
/// - Property change event validation
/// - State transition verification (progress → status)
/// </remarks>
public class ToDoItemViewModelTest
{
    /// <summary>
    /// Tests that creating a ToDoItemViewModel from a ToDoItem model correctly 
    /// maps all properties to the ViewModel.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - All basic properties (ID, Title, Description) are copied
    /// - Enum properties (Priority) are converted correctly
    /// - DateTime properties (DueDate, CreatedDate) are preserved
    /// - Related Category is wrapped in CategoryViewModel
    /// - CompletedDate remains null when the model is not completed yet
    /// </remarks>
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

    /// <summary>
    /// Tests that when the source model has a null Category, the ViewModel 
    /// provides a default "Uncategorized" category instead of crashing.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - ViewModel creates a CategoryViewModel instead of using null
    /// - Default category matches CategoryViewModel.Empty
    /// - UI can safely bind to Category without null-checks
    /// </remarks>
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

    /// <summary>
    /// Tests that CurrentStatus is correctly calculated based on progress percentage.
    /// </summary>
    /// <param name="progress">The progress percentage (0, 50, or 100)</param>
    /// <param name="expectedStatus">The expected status (NotStarted, InProgress, or Done)</param>
    /// <remarks>
    /// Expected behavior:
    /// - 0% progress → NotStarted
    /// - 1-99% progress → InProgress
    /// - 100% progress → Done
    /// </remarks>
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

    /// <summary>
    /// Tests that items past their due date are marked as Overdue,
    /// even if work has begun (progress > 0).
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Due date in the past → Overdue status
    /// - Progress value does not affect overdue detection
    /// - Overdue takes precedence over InProgress
    /// </remarks>
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

    /// <summary>
    /// Tests that completed items (100% progress) are always marked Done,
    /// overriding overdue status when an item is finished.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - 100% progress → Done status (regardless of due date)
    /// - Completed items never show overdue after completion
    /// - Done status has the highest priority in status logic
    /// </remarks>
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

    /// <summary>
    /// Tests that the ToToDoItem() method correctly converts the ViewModel 
    /// back to a database model, including ID and foreign key mapping.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - All properties are copied back to the database model
    /// - The CategoryViewModel.Id becomes CategoryId (foreign key)
    /// - DateTime values are preserved
    /// - Enum values are converted to integer representation
    /// </remarks>
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

    /// <summary>
    /// Tests that CloneToDoItemViewModel() creates a truly independent copy
    /// that can be modified without affecting the original.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Clone is a new instance (different reference)
    /// - Changes to the clone don't affect the original ViewModel
    /// - Category uses default "Uncategorized" if the original had null
    /// </remarks>
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

    /// <summary>
    /// Tests that PropertyChanged events are raised for modified properties.
    /// Verifies INotifyPropertyChanged implementation for UI data binding.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - PropertyChanged fires when Title changes
    /// - PropertyChanged fires when Progress changes
    /// - Property names are correctly reported in PropertyChangedEventArgs
    /// </remarks>
    [Fact]
    public void ToDoItemViewModel_PropertyChanges_NotifyCorrectly()
    {
        // Arrange
        var model = new ToDoItem { Title = "Test" };
        var viewModel = new ToDoItemViewModel(model);
        var titleChanged = false;
        var progressChanged = false;

        viewModel.PropertyChanged += (_, e) =>
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

    /// <summary>
    /// Tests that changing Progress triggers a status change notification.
    /// Verifies that CurrentStatus is a derived property that notifies changes.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - PropertyChanged fires with CurrentStatus when progress changes
    /// - New status matches the updated progress value
    /// - UI can react to status changes without manual status checks
    /// </remarks>
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

        viewModel.PropertyChanged += (_, e) =>
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

    /// <summary>
    /// Tests that setting Progress to 100% automatically sets CompletedDate
    /// to the current time.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - CompletedDate is set when progress reaches 100%
    /// - CompletedDate reflects current time (within one second)
    /// - CompletedDate is null for incomplete items
    /// </remarks>
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

    /// <summary>
    /// Tests that reducing progress from 100% clears the CompletedDate,
    /// indicating the item is no longer finished.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - CompletedDate becomes null when progress drops below 100%
    /// - Item correctly reverts to incomplete status
    /// - CompletedDate state is properly tracked
    /// </remarks>
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

    /// <summary>
    /// Tests that the Priority property can be set to all valid Priority values.
    /// </summary>
    /// <param name="priority">The priority value to test (Low, Medium, High)</param>
    /// <remarks>
    /// Expected behavior:
    /// - All Priority enum values are accepted
    /// - Set value matches retrieved value
    /// - Priority property works correctly for UI binding
    /// </remarks>
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

    /// <summary>
    /// Tests that changing DueDate updates CurrentStatus appropriately.
    /// Specifically, verifies overdue status calculation when the due date changes.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Moving due date to past triggers Overdue status
    /// - Status reflects both progress and due date relationship
    /// - DueDate changes properly propagate through status logic
    /// </remarks>
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