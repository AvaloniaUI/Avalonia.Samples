using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Desktop.Services;

/// <summary>
/// Desktop-specific implementation of the database service.
/// Uses the OS-specific local application data folder (e.g., %LOCALAPPDATA% on Windows,
/// ~/.local/share on Linux, ~/Library/Application Support on macOS) to store the SQLite database.
/// </summary>
public class DesktopDbService : IDatabaseService
{
    /// <summary>
    /// Gets the absolute path to the SQLite database file on the local machine.
    /// Creates the app-specific subdirectory in the local app data folder if it doesn't exist.
    /// </summary>
    /// <returns>The full path to the database file (e.g., C:\Users\Name\AppData\Local\AdvancedToDoList\fileName.db on Windows)</returns>
    public string GetDatabasePath()
    {
        string localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string appFolder = Path.Combine(localAppData, "AdvancedToDoList");
    
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        return Path.Combine(appFolder, "todo.db");
    }
    
    /// <summary>
    /// Saves database changes.
    /// Currently, a no-op implementation as SQLite operations are handled directly
    /// by the database connection and don't require explicit save operations.
    /// </summary>
    public Task SaveAsync() => Task.CompletedTask;
}