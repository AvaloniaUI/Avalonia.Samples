using System;
using System.Threading;
using AdvancedToDoList;
using AdvancedToDoList.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AdvancedToDoListTests;

public class TestBase : IDisposable
{
    public TestBase()
    {
        // Set up a mock service provider
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IDbService>(new DesignDbService());
        serviceCollection.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
        
        App.Services = serviceCollection.BuildServiceProvider();

        // Ensure we have a SynchronizationContext that doesn't hang in tests.
        // We can use a simple one or just leave it null so it doesn't default to AvaloniaSynchronizationContext
        // if we change the VM code to handle null.
        // For now, let's just make sure App.Services is set.
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
