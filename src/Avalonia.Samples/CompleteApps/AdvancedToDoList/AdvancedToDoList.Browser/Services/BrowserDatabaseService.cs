using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Browser.Services;

public partial class BrowserDbService : IDatabaseService
{
    public string GetDatabasePath()
    {
        // This path must match the DB_FILENAME in sqlite-storage.js
        return "todo.db";
    }

    public async Task SaveAsync()
    {
        try
        {
            await SaveDbJs();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    [JSImport("globalThis.saveDatabase")]
    private static partial Task SaveDbJs();
}