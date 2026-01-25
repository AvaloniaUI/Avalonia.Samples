using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service to work with the Database.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Returns the full path to the database file.
    /// </summary>
    string GetDatabasePath();
    
    /// <summary>
    /// Persists data to permanent storage (required for Browser/WASM).
    /// </summary>
    Task SaveAsync();
}
