using System;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Browser.Services;

public partial class BrowserSettingsStorageService : ISettingsStorageService
{
    [JSImport("setItem", "storage")]
    private static partial void SetItem(string key, string value);

    [JSImport("getItem", "storage")]
    private static partial string? GetItem(string key);

    private static string Identifier { get; } = "Avalonia.Samples.AdvancedToDoList";

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

    private async Task InitializeAsync()
    {
        const string storageJsLocation = "../storage.js";
        await JSHost.ImportAsync("storage", storageJsLocation);
    }
}