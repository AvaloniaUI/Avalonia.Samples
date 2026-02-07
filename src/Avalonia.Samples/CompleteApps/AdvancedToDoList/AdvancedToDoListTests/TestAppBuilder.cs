using Avalonia;
using Avalonia.Headless;
using AdvancedToDoList;

using Microsoft.Extensions.DependencyInjection;
using AdvancedToDoList.Services;

//  Assembly-level attribute that tells Xunit how to initialize Avalonia for tests.
//  Specifies that TestAppBuilder.BuildAvaloniaApp() should be called
//  once before any tests run to set up the Avalonia application.
//
//  Why is this needed?
//  - Avalonia requires initialization before any UI operations can occur
//  - Tests need a headless (no visible window) Avalonia environment
//  - This ensures consistent Avalonia setup across all test runs
//  
//  What happens when tests run?
//  1. Xunit discovers this assembly-level attribute
//  2. Calls TestAppBuilder.BuildAvaloniaApp() once
//  3. Sets up headless Avalonia platform
//  4. Initializes the App class
//  5. Tests can now use Avalonia controls and ViewModels
//  
//  Headless Platform:
//  - No visible windows or UI rendering
//  - Fast execution without GUI dependencies
//  - Perfect for unit and integration testing
//  - Simulates Avalonia behavior without display
[assembly: AvaloniaTestApplication(typeof(AdvancedToDoListTests.TestAppBuilder))]

namespace AdvancedToDoListTests;

/// <summary>
/// Builds and configures the Avalonia application for testing.
/// Provides a headless (invisible) Avalonia environment for test execution.
/// </summary>
/// <remarks>
/// Why use a headless app for testing?
/// - Tests run faster without UI rendering overhead
/// - No visible windows disturb the test runner
/// - Can test ViewModels and business logic without full UI
/// - Runs in CI/CD environments without display capabilities
/// 
/// How it works:
/// - Configures the App class as the Avalonia application
/// - Uses Avalonia.Headless platform instead of desktop/mobile/web
/// - No visual rendering occurs, just logical UI operations
/// - Services and ViewModels work exactly as in production
/// 
/// Integration with Xunit:
/// - Called automatically via AvaloniaTestApplication attribute
/// - Runs once before all tests in the assembly
/// - Provides consistent test environment
/// 
/// Note: This approach differs from UI automation tools like Selenium.
/// It tests ViewModels and business logic at the code level, not through UI automation.
/// </remarks>
public class TestAppBuilder
{
    /// <summary>
    /// Builds and configures the Avalonia application for test execution.
    /// Sets up a headless Avalonia platform with the main App class.
    /// </summary>
    /// <returns>
    /// Configured AppBuilder ready for test execution.
    /// The builder configures the App class with headless platform options.
    /// </returns>
    /// <remarks>
    /// Configuration details:
    /// - Uses AdvancedToDoList.App as the application class
    /// - Avalonia.Headless platform: No visual rendering, just logic
    /// 
    /// Called automatically by:
    /// - AvaloniaTestApplication assembly attribute
    /// - Before any tests run
    /// - Once per test session
    /// </remarks>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
