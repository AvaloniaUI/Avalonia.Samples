using AdvancedToDoList.ViewModels;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for MainViewModel.
/// Tests the initialization of child ViewModels to ensure proper
/// composition and dependency setup when MainViewModel is constructed.
/// </summary>
/// <remarks>
/// Why test MainViewModel?
/// - Validates that child ViewModels are properly instantiated on construction
/// - Ensures key ViewModels (Categories, To-Do Items, Settings) are available
/// - Confirms the main ViewModel correctly orchestrates its subcomponents
/// 
/// Test categories covered:
/// - Constructor behavior: Initialization of child ViewModels
/// - Null safety: Verifying no ViewModels are accidentally omitted
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Synchronous test setup with Dispatcher job flushing for Avalonia compatibility
/// </remarks>
public class MainViewModelTest : TestBase
{
    /// <summary>
    /// Tests that the MainViewModel constructor initializes all child ViewModels.
    /// Verifies that CategoriesViewModel, ToDoItemsViewModel, and SettingsViewModel
    /// are properly instantiated and not null after construction.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - All child ViewModels are instantiated (not null)
    /// - No exceptions thrown during construction
    /// - Dispatcher jobs are processed before assertions
    /// </remarks>
    [AvaloniaFact]
    public async Task MainViewModel_Constructor_InitializesChildViewModels()
    {
        // Arrange & Act - Create the MainViewModel and flush dispatcher jobs
        var vm = new MainViewModel();
        Dispatcher.UIThread.RunJobs();
        await Task.Delay(100); // wait some milliseconds to process the I/O operations
        
        // Assert - Verify all child ViewModels are initialized
        Assert.NotNull(vm.CategoriesViewModel);
        Assert.NotNull(vm.ToDoItemsViewModel);
        Assert.NotNull(vm.SettingsViewModel);
    }
}
