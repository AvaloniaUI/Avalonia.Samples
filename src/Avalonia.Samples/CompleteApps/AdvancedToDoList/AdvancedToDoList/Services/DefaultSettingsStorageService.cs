using System;
using System.IO;
using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// Default file system-based settings storage service.
/// Persists application settings to local disk using JSON format.
/// On Browser platform, it behaves as a no-op due to sandbox restrictions.
/// </summary>
public sealed class DefaultSettingsStorageService : ISettingsStorageService
{
    /// <summary>
    /// Gets the settings directory path for storing user configuration.
    /// Uses OS-specific local application data directory.
    /// Creates dedicated folder for this application to avoid conflicts.
    /// </summary>
    private static string SettingsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Avalonia.Samples.AdvancedToDoList");
    
    /// <summary>
    /// Gets the full path to the settings file.
    /// Combines settings directory with JSON filename for configuration storage.
    /// </summary>
    private static string SettingsFile => Path.Combine(SettingsDirectory, "Settings.json");

    /// <inheritdoc />
    public async Task<string?> ReadAsync()
    {
        try
        {
            // Browser has no access to file system due to sandbox restrictions
            if (OperatingSystem.IsBrowser()) return null;
            
            // Return null if settings file doesn't exist yet
            if (!File.Exists(SettingsFile)) return null;
            
            // Read and return JSON content from settings file
            return await File.ReadAllTextAsync(SettingsFile);
        }
        catch
        {
            // In production, consider logging any exceptions for debugging
            return null;
        }
    }

    /// <inheritdoc />
    public async Task WriteAsync(string json)
    {
        try
        {
            // Browser has no access to file system due to its sandbox status
            if (OperatingSystem.IsBrowser()) return;
            
            // Create directory for this App if it doesn't exist
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }
            
            // Save the provided data into our settings file
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch
        {
            // For this sample, we ignore exceptions
            // In production, consider logging exceptions for debugging
        }
    }
}