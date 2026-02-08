
using Avalonia;
using Avalonia.Headless;
using SharedControlsTests;

// Test application configuration for headless Avalonia testing.
// Provides a specialized application instance that runs without a GUI,
// enabling unit tests to exercise UI components in an isolated environment.
//
// Why headless testing?
// - Enables UI component testing without requiring a display/server
// - Runs faster than GUI tests by avoiding window creation overhead
// - Allows testing on CI/CD servers without desktop environment
// - Supports property inspection and control manipulation
// 
// This class is used with [assembly: AvaloniaTestApplication] to configure
// the test framework to use this application instance instead of a regular one.
// 
// Test capabilities enabled:
// - HamburgerMenu interaction and rendering tests
// - Control initialization and property verification
// - UI event simulation and verification
// - Visual tree inspection and validation

[assembly: AvaloniaTestApplication(typeof(TestAppliction))]
namespace SharedControlsTests;

/// <summary>
/// Headless test application for Avalonia UI components.
/// Configures the application to run without displaying any windows,
/// allowing unit tests to interact with UI controls programmatically.
/// </summary>
/// <remarks>
/// Key characteristics:
/// - Uses Headless platform backend (no windowing system required)
/// - Provides isolated testing environment for UI components
/// - Compatible with Avalonia.Headless.XUnit test framework
/// - Supports all standard Avalonia controls and features
/// 
/// Configuration details:
/// - UseHeadless() initializes the headless platform
/// - AvaloniaHeadlessPlatformOptions allows platform-specific tuning
/// - No display output needed - all rendering is virtual
/// - Tests can create windows and controls programmatically
/// </remarks>
public class TestAppliction : Application
{
    /// <summary>
    /// Configures and returns the application builder for testing.
    /// Sets up the headless platform with default options.
    /// </summary>
    /// <returns>AppBuilder configured for headless testing</returns>
    /// <remarks>
    /// This method is the entry point for test application initialization.
    /// It's called by the test framework to create the application instance
    /// used during test execution.
    /// 
    /// The headless platform:
    /// - Simulates UI operations without visible windows
    /// - Allows control manipulation and property verification
    /// - Supports all Avalonia features needed for testing
    /// </remarks>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestAppliction>()
         .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}