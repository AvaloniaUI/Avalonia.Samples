using System.Threading.Tasks;

namespace AdvancedToDoList.Services;

public class DesignDbService : IDbService
{
    public string GetDatabasePath()
    {
        return ":memory:";
    }

    public Task SaveAsync()
    {
        throw new System.NotSupportedException();
    }
}