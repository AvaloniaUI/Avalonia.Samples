using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service interface for database management and persistence operations.
/// Provides platform-agnostic abstraction for database file handling.
/// Enables different implementations for desktop, mobile, and web platforms.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Gets the full file path to the SQLite database.
    /// Returns platform-specific path where the database file should be located.
    /// Used by DatabaseHelper for establishing database connections.
    /// </summary>
    /// <returns>Full file system path to the SQLite database file</returns>
    string GetDatabasePath();
    
    /// <summary>
    /// Persists in-memory database changes to permanent storage.
    /// Essential for platforms like Browser/WASM where data needs explicit synchronization.
    /// No-op on platforms with direct file system access (desktop/mobile).
    /// </summary>
    /// <returns>Task representing the save operation</returns>
    Task SaveAsync();
}
