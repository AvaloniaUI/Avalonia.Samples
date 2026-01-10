using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

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
