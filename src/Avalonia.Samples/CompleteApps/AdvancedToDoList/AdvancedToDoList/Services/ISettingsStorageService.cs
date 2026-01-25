using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that handles Settings related I/O operations.
/// </summary>
public interface ISettingsStorageService
{
    /// <summary>
    /// Reads the settings JSON string or returns null if unavailable.
    /// </summary>
    Task<string?> ReadAsync();

    /// <summary>
    /// Writes the provided settings JSON string.
    /// No-op if writing is not supported on the current platform.
    /// </summary>
    Task WriteAsync(string json);
}
