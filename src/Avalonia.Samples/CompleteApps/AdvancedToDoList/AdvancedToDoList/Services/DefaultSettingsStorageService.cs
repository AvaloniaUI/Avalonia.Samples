using System;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// Default file system-based storage. On Browser, it behaves as a no-op.
/// </summary>
public sealed class DefaultSettingsStorageService : ISettingsStorageService
{
    // The Settings directory to store user settings. We use "SpecialFolder.LocalApplicationData" as the base dir. 
    private static string SettingsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Avalonia.Samples.AdvancedToDoList");
    
    // The file name of the settings. 
    private static string SettingsFile => Path.Combine(SettingsDirectory, "Settings.json");

    /// <inheritdoc />
    public async Task<string?> ReadAsync()
    {
        try
        {
            // Browser has no access to the file system due to its sandbox status.
            if (OperatingSystem.IsBrowser()) return null;
            
            // If the file exists, return its content as string.
            if (!File.Exists(SettingsFile)) return null;
            return await File.ReadAllTextAsync(SettingsFile);
        }
        catch
        {
            // In production, consider logging any exceptions. 
            return null;
        }
    }

    /// <inheritdoc />
    public async Task WriteAsync(string json)
    {
        try
        {
            // Browser has no access to the file system due to its sandbox status.
            if (OperatingSystem.IsBrowser()) return;
            
            // If the dir for this App doesn't exist, create it. 
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }
            
            // Save the provided data into our settings file. 
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch
        {
            // For this sample, we ignore exceptions. 
            // In production, consider logging the exceptions.
        }
    }
}