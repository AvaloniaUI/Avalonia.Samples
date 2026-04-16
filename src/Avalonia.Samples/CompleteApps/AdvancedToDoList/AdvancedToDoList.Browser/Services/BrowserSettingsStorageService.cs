using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Browser.Services;

/// <summary>
/// Browser-specific implementation of settings storage.
/// Persists application settings using JavaScript's localStorage (via WebAssembly interop),
/// with a single key per app to store serialized JSON settings.
/// </summary>
public partial class BrowserSettingsStorageService : ISettingsStorageService
{
    /// <summary>
    /// Imports and calls the JavaScript <c>setItem</c> function from the <c>sqlite-storage</c> module.
    /// Used to persist settings data in the browser's localStorage.
    /// </summary>
    /// <param name="key">The storage key (application identifier)</param>
    /// <param name="value">The JSON string to store</param>
    [JSImport("setItem", "sqlite-storage")]
    private static partial void SetItem(string key, string value);

    /// <summary>
    /// Imports and calls the JavaScript <c>getItem</c> function from the <c>sqlite-storage</c> module.
    /// Retrieves previously stored settings from the browser's localStorage.
    /// </summary>
    /// <param name="key">The storage key (application identifier)</param>
    /// <returns>The stored JSON string, or <c>null</c> if no data exists</returns>
    [JSImport("getItem", "sqlite-storage")]
    private static partial string? GetItem(string key);

    /// <summary>
    /// Unique identifier used as the storage key in localStorage.
    /// Matches the assembly namespace to avoid naming collisions across apps.
    /// </summary>
    private static string Identifier { get; } = "Avalonia.Samples.AdvancedToDoList";

    /// <summary>
    /// Reads saved settings from browser storage.
    /// Initializes the JavaScript module on first use and retrieves the stored JSON settings.
    /// </summary>
    /// <returns>The deserialized settings JSON string, or <c>null</c> on error or if not found</returns>
    public async Task<string?> ReadAsync()
    {
        try
        {
            await InitializeAsync();
            return GetItem(Identifier);
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Saves settings to browser storage by serializing them to JSON.
    /// Initializes the JavaScript module on first use and stores the JSON under the app's identifier.
    /// </summary>
    /// <param name="json">The JSON string representation of settings to save</param>
    public async Task WriteAsync(string json)
    {
        try
        {
            await InitializeAsync();
            SetItem(Identifier, json);
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
    }

    /// <summary>
    /// Loads the JavaScript module <c>sqlite-storage.js</c> dynamically in the browser environment.
    /// Uses a relative path to locate the module—must match the actual file location in your project.
    /// </summary>
    private async Task InitializeAsync()
    {
        const string storageJsLocation = "../sqlite-storage.js";
        await JSHost.ImportAsync("sqlite-storage", storageJsLocation);
    }
}