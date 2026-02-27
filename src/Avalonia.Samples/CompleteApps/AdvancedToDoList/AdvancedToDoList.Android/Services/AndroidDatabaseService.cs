using System;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Android.Services;

/// <summary>
/// Android-specific implementation of the database service.
/// This class handles platform-specific database path resolution for Android.
/// </summary>
public class AndroidDbService : IDatabaseService
{
    /// <summary>
    /// Gets the absolute path to the SQLite database file on the device.
    /// Uses Android's personal/storage directory (typically app's private storage).
    /// </summary>
    /// <returns>The full path to the database file (e.g., /data/data/[app_id]/files/fileName.db)</returns>
    public string GetDatabasePath()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(path, "todo.db");
    }
    
    /// <summary>
    /// Saves database changes.
    /// Currently, a no-op implementation as SQLite operations are handled directly
    /// by the database connection and don't require explicit save operations.
    /// </summary>
    public Task SaveAsync() => Task.CompletedTask;
}
