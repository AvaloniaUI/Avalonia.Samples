using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

public interface IDbService
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
