using System;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Services;

public class DesignDbService : IDbService
{
    public string GetDatabasePath()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "ToDoItems.db");
        if (File.Exists(dbPath))
        {
            return dbPath;
        }

        // Fallback for cases where the file is not in the output directory (e.g. during some designer scenarios)
        return ":memory:";
    }
    
    public Task SaveAsync()
    {
        return Task.CompletedTask;
    }
}