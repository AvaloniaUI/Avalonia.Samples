using AdvancedToDoList.ViewModels;
using Xunit;

namespace AdvancedToDoListTests.ViewModelTests;

/// <summary>
/// Unit tests for the SettingsViewModel.
/// Tests the initialization of settings collection and theme variant options.
/// </summary>
/// <remarks>
/// Why test SettingsViewModel?
/// - Validates that settings are properly loaded on construction
/// - Ensures theme variants are available and correctly configured
/// - Confirms UI-adjacent configuration data is accessible and complete
/// 
/// Test categories covered:
/// - Constructor behavior: Initialization of settings collection and theme options
/// - Theme availability: Verification of expected theme variants (Default, Dark, Light)
/// - Data integrity: Ensuring no missing or null configuration values
/// 
/// Testing patterns used:
/// - Arrange-Act-Assert (AAA) pattern for clarity
/// - Collection assertion patterns for theme variant validation
/// - Independent test isolation (no shared state between tests)
/// </remarks>
public class SettingsViewModelTest : TestBase
{
    /// <summary>
    /// Tests that the SettingsViewModel constructor initializes all core properties correctly.
    /// Verifies the settings collection is populated and theme variant options are available.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// - The settings collection is initialized (not null)
    /// - the available theme variants list contains the three available options
    /// - Required themes (Default, Dark, Light) are present in the list
    /// - No exceptions thrown during construction
    /// 
    /// Theme variant rationale:
    /// - "Default": System-default theme behavior
    /// - "Dark": Dark mode for reduced eye strain in low light
    /// - "Light": Light mode for maximum contrast in bright environments
    /// </remarks>
    [Fact]
    public void SettingsViewModel_Constructor_InitializesCorrectly()
    {
        // Arrange & Act - Construct the ViewModel
        var vm = new SettingsViewModel();

        // Assert - Verify all properties are initialized correctly
        Assert.NotNull(vm.Settings);
        Assert.NotEmpty(vm.AvailableThemeVariants);
        Assert.Contains("Default", vm.AvailableThemeVariants);
        Assert.Contains("Dark", vm.AvailableThemeVariants);
        Assert.Contains("Light", vm.AvailableThemeVariants);
    }
}
