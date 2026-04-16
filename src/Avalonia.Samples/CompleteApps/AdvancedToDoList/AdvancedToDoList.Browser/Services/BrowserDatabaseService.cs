using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Browser.Services;

/// <summary>
/// Browser-specific implementation of the database service.
/// Uses WebAssembly-to-JavaScript interop to communicate with a client-side SQLite database
/// managed by the JavaScript module (sqlite-storage.js).
/// </summary>
public partial class BrowserDbService : IDatabaseService
{
    /// <summary>
    /// Gets the logical database name used by the JavaScript-side SQLite storage.
    /// This must match the DB_FILENAME constant defined in sqlite-storage.js to ensure
    /// consistency between the .NET and JavaScript layers.
    /// </summary>
    /// <returns>The logical database identifier ("fileName.db")</returns>
    public string GetDatabasePath()
    {
        // This path must match the DB_FILENAME in sqlite-storage.js
        return "todo.db";
    }

    /// <summary>
    /// Saves database changes by delegating to the JavaScript implementation.
    /// Since browser environments can't directly access the file system,
    /// this method invokes the corresponding JavaScript function via WebAssembly interop.
    /// </summary>
    public async Task SaveAsync()
    {
        try
        {
            await SaveDbJs();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// WebAssembly-to-JavaScript import declaration.
    /// Maps to the global JavaScript function <c>globalThis.saveDatabase</c> defined in sqlite-storage.js.
    /// This partial method enables calling JS from C# in a type-safe way.
    /// </summary>
    [JSImport("globalThis.saveDatabase")]
    private static partial Task SaveDbJs();
}