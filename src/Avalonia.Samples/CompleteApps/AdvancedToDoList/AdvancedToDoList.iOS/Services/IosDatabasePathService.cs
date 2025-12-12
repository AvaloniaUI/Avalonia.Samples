using System;
using System.IO;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.iOS.Services;

public class IosDbService : IDbService
{
    public string GetDatabasePath()
    {
        string docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string libFolder = Path.Combine(docFolder, "..", "Library");
        
        if (!Directory.Exists(libFolder))
        {
            Directory.CreateDirectory(libFolder);
        }

        return Path.Combine(libFolder, "todo.db");
    }
    
    public Task SaveAsync() => Task.CompletedTask;
}
