using AdvancedToDoList;
using AdvancedToDoList.Services;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoListTests;

/// <summary>
/// Base class for all test classes in the AdvancedToDoList project.
/// Sets up common test infrastructure including dependency injection,
/// synchronization context, and mock services.
/// </summary>
/// <remarks>
/// Why use TestBase?
/// - Consistent test environment across all test classes
/// - Automatic setup of dependency injection container
/// - Mock services for isolated testing without real database
/// - Proper synchronization context for Avalonia UI operations
/// 
/// What it provides:
/// - Design-time database service (DesignDbService)
/// - Settings storage service (DefaultSettingsStorageService)
/// - SynchronizationContext for UI thread operations
/// - Service provider accessible via App.Services
/// 
/// How to use it:
/// Inherit from TestBase in your test class:
/// <code>
/// public class MyViewModelTests : TestBase
/// {
///     [Fact]
///     public void TestSomething()
///     {
///         // Services are already configured
///         var dbService = App.Services.GetService&lt;IDatabaseService&gt;();
///         // Your test code here
///     }
/// }
/// </code>
/// </remarks>
public class TestBase : IDisposable
{
    /// <summary>
    /// Constructor that sets up the test environment.
    /// Runs before each test method to ensure a clean state.
    /// </summary>
    public TestBase()
    {
        // Ensure we have a SynchronizationContext for tests that use ObserveOn(syncContext)
        // Avalonia requires a synchronization context for UI operations like property notifications
        if (SynchronizationContext.Current == null)
        {
            SynchronizationContext.SetSynchronizationContext(new AvaloniaSynchronizationContext());
        }

        // Set up a mock service provider with test-specific implementations
        // This allows ViewModels to work without needing real database connections
        var serviceCollection = new ServiceCollection();
        
        // Use DesignDbService for in-memory testing without file system dependencies
        serviceCollection.AddSingleton<IDatabaseService>(new DesignDbService());
        
        // Use DefaultSettingsStorageService (in-memory for tests, no file system)
        serviceCollection.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
        
        // Build the service provider and make it available globally via App.Services
        // ViewModels can now resolve services through dependency injection
        App.Services = serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Cleanup method that runs after each test completes.
    /// Implement IDisposable for proper resource management.
    /// </summary>
    /// <remarks>
    /// Currently performs minimal cleanup but is available for future use.
    /// Can be extended to clean up test databases, reset state, etc.
    /// </remarks>
    public void Dispose()
    {
        // Cleanup if needed
        // Currently no specific cleanup required
        // This method is available for future test cleanup needs
    }
}
