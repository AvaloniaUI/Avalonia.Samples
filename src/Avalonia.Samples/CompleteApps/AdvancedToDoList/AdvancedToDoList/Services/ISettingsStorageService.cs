using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service interface for persisting application settings.
/// Provides platform-agnostic abstraction for settings storage operations.
/// Enables different implementations for desktop, mobile, and browser platforms.
/// </summary>
public interface ISettingsStorageService
{
    /// <summary>
    /// Asynchronously reads application settings from persistent storage.
    /// Returns the JSON string containing settings data, or null if no settings exist.
    /// </summary>
    /// <returns>Settings JSON string or null if unavailable/unreadable</returns>
    Task<string?> ReadAsync();

    /// <summary>
    /// Asynchronously writes application settings to persistent storage.
    /// Persists the provided JSON string to the appropriate storage location.
    /// May be a no-op on platforms with restricted write access (e.g., browser sandbox).
    /// </summary>
    /// <param name="json">The JSON string containing application settings to persist</param>
    Task WriteAsync(string json);
}
