using System;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// Default file system based storage. On Browser it behaves as no-op.
/// </summary>
public sealed class DefaultSettingsStorageService : ISettingsStorageService
{
    private static string SettingsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Avalonia.Samples.AdvancedToDoList");
    private static string SettingsFile => Path.Combine(SettingsDirectory, "Settings.json");

    public async Task<string?> ReadAsync()
    {
        try
        {
            if (OperatingSystem.IsBrowser()) return null;
            if (!File.Exists(SettingsFile)) return null;
            return await File.ReadAllTextAsync(SettingsFile);
        }
        catch
        {
            return null;
        }
    }

    public async Task WriteAsync(string json)
    {
        try
        {
            if (OperatingSystem.IsBrowser()) return;
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch
        {
            // ignore
        }
    }
}