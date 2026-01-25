using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Desktop.Services;

public class DesktopDbService : IDatabaseService
{
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
    
    public Task SaveAsync() => Task.CompletedTask;
}
