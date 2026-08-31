using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for ManageToDoItemsViewModel.
/// Tests the initialization of the to-do items management ViewModel,
/// property setters, and default configuration of sorting/filtering behavior.
/// </summary>
/// <remarks>
/// Why test ManageToDoItemsViewModel?
/// - Validates that all core properties (items list, filters, sort expressions)
///   are correctly initialized to expected defaults
/// - Ensures proper state management for user-facing controls (filtering, sorting, selection)
/// - Confirms ViewModel integration with Avalonia's dispatcher for async operations
/// 
/// Test categories covered:
/// - Constructor behavior: Initialization of to-do items and filter/sort configuration
/// - Property setters: Verification of FilterString, ShowAlsoCompletedItems, SelectedToDoItem
/// - Sorting Defaults: Confirmation of expected sort expression chain
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Constructor injection for test output (xUnit ITestOutputHelper)
/// - Dispatcher job flushing for deterministic Avalonia test execution
/// - Independent test isolation (no shared state between tests)
/// </remarks>
public class ManageToDoItemsViewModelTest : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the ManageToDoItemsViewModelTest class.
    /// </summary>
    /// <param name="testOutputHelper">The xUnit test output helper for logging.</param>
    public ManageToDoItemsViewModelTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests that the ManageToDoItemsViewModel constructor initializes all
    /// core properties to their expected default values.
    /// Verifies the to-do items collection, filtering options, and sorting configuration.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - The To-Do items collection is initialized (not null)
    /// - FilterString is null (no active filter)
    /// - ShowAlsoCompletedItems is false (completed items hidden by default)
    /// - Sort expressions follow the expected priority order: DueDate → Priority → Title
    /// 
    /// Default sort order rationale:
    /// - SortExpression1: Sort by due date (most urgent tasks first)
    /// - SortExpression2: Sort by priority (within same due date)
    /// - SortExpression3: Sort by title (alphabetical tie-breaker)
    /// </remarks>
    [AvaloniaFact]
    public async Task ManageToDoItemsViewModel_Constructor_InitializesCorrectly()
    {
        // We use the logging here just to show you how you can get some output from your unit test if needed.
        // Log test start for debugging
        _testOutputHelper.WriteLine("Starting ManageToDoItemsViewModel constructor test...");

        // Arrange & Act - Construct the ViewModel, await initialization and flush dispatcher jobs
        var vm = new ManageToDoItemsViewModel();
        await vm.InitializationTask;
        Dispatcher.UIThread.RunJobs();

        // Log values before assertions for easier debugging
        _testOutputHelper.WriteLine($"Items count: {vm.ToDoItems.Count}");
        _testOutputHelper.WriteLine($"Filter: '{vm.FilterString ?? "null"}', Show completed: {vm.ShowAlsoCompletedItems}");
        _testOutputHelper.WriteLine($"Sort1: {vm.SortExpression1}, Sort2: {vm.SortExpression2}, Sort3: {vm.SortExpression3}");

        // Assert - Verify all properties match expected defaults
        Assert.NotNull(vm.ToDoItems);
        Assert.Null(vm.FilterString);
        Assert.False(vm.ShowAlsoCompletedItems);
        Assert.Equal(ToDoItemsSortExpression.SortByDueDateExpression, vm.SortExpression1);
        Assert.Equal(ToDoItemsSortExpression.SortByPriorityExpression, vm.SortExpression2);
        Assert.Equal(ToDoItemsSortExpression.SortByTitleExpression, vm.SortExpression3);
    }

    /// <summary>
    /// Tests that key properties of ManageToDoItemsViewModel can be successfully
    /// modified and their new values are properly stored and reflected.
    /// Verifies basic property setter behavior for user-filter controls.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - FilterString accepts and stores string values
    /// - ShowAlsoCompletedItems accepts and stores boolean toggles
    /// - SelectedToDoItem accepts and stores a new to-do item reference
    /// - The selected item's properties (e.g., Title) remain accessible
    /// </remarks>
    [AvaloniaFact]
    public async Task ManageToDoItemsViewModel_Properties_CanBeSet()
    {
        // Arrange - Create the ViewModel
        var vm = new ManageToDoItemsViewModel();
        
        // Await initialization and flush dispatcher jobs to ensure items are loaded
        await vm.InitializationTask;
        Dispatcher.UIThread.RunJobs();
        
        var selectedItem = vm.ToDoItems.First();

        // Act - Set properties to new values
        vm.FilterString = selectedItem.Title;
        vm.ShowAlsoCompletedItems = true;
        vm.SelectedToDoItem = selectedItem;

        // Assert - Verify all properties were updated correctly
        Assert.Equal(selectedItem.Title, vm.FilterString);
        Assert.True(vm.ShowAlsoCompletedItems);
        Assert.NotNull(vm.SelectedToDoItem);
        Assert.Equal(selectedItem, vm.SelectedToDoItem);
    }
}
