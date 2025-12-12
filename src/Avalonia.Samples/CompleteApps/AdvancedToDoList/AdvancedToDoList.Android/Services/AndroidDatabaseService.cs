using System;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Android.Services;

public class AndroidDbService : IDbService
{
    public string GetDatabasePath()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(path, "todo.db");
    }
    
    public Task SaveAsync() => Task.CompletedTask;
}
