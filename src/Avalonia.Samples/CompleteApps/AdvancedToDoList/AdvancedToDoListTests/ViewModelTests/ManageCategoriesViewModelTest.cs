using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for ManageCategoriesViewModel.
/// Tests the initialization of the categories management ViewModel,
/// category selection behavior, and basic interaction patterns.
/// </summary>
/// <remarks>
/// Why test ManageCategoriesViewModel?
/// - Validates that the ViewModel properly initializes its categories collection
/// - Ensures proper state management for the currently selected category
/// - Confirms that the ViewModel integrates correctly with Avalonia's dispatcher
/// 
/// Test categories covered:
/// - Constructor behavior: Initialization of categories and selection state
/// - Property setters: Verification of SelectedCategory assignment
/// - Dispatcher interaction: Proper handling of async initialization jobs
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Dispatcher job flushing for deterministic Avalonia test execution
/// - Independent test isolation (no shared state between tests)
/// </remarks>
public class ManageCategoriesViewModelTest : TestBase
{
    /// <summary>
    /// Tests that the ManageCategoriesViewModel constructor properly initializes
    /// the categories collection and resets the selected category.
    /// Verifies that the ViewModel is ready to display categories after construction.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - The Categories collection is initialized (not null)
    /// - SelectedCategory is null (no category selected by default)
    /// - Any background initialization (e.g., LoadDataAsync) completes before assertions
    /// 
    /// Note: This test may trigger asynchronous data loading via DatabaseHelper.
    /// A delay is included to ensure initialization completes before assertions.
    /// </remarks>
    [AvaloniaFact]
    public async Task ManageCategoriesViewModel_Constructor_InitializesCorrectly()
    {
        // Arrange & Act - Construct the ViewModel and flush dispatcher jobs
        var vm = new ManageCategoriesViewModel();
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);

        // Assert - Verify categories collection is initialized and no item is selected
        Assert.NotNull(vm.Categories);
        Assert.Null(vm.SelectedCategory);
    }

    /// <summary>
    /// Tests that the SelectedCategory property can be assigned a new value
    /// and correctly stores the reference.
    /// Verifies basic property setter behavior for category selection.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - Setting SelectedCategory to a valid CategoryViewModel succeeds
    /// - The same instance reference is preserved (not copied or wrapped)
    /// - No exceptions are thrown during assignment
    /// </remarks>
    [AvaloniaFact]
    public async Task ManageCategoriesViewModel_SelectedCategory_CanBeSet()
    {
        // Arrange - Create the ViewModel and a test category
        var vm = new ManageCategoriesViewModel();

        // Flush dispatcher jobs to ensure initialization completes
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(500);

        // Act - Assign the test category to SelectedCategory
        vm.SelectedCategory = vm.Categories.First();

        // Assert - Verify the assignment preserved the instance reference
        Assert.Same(vm.Categories[0], vm.SelectedCategory);
    }
}
