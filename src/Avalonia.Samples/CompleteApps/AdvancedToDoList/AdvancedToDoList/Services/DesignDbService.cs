using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

public class DesignDbService : IDatabaseService
{
    /// <inheritdoc />
    public string GetDatabasePath()
    {
        // For the designer, we use an in-memory DB. 
        // See: https://www.sqlite.org/inmemorydb.html 
        return ":memory:";
    }
    
    /// <inheritdoc />
    public Task SaveAsync()
    {
        // The designer will not save anything.
        return Task.CompletedTask;
    }
}