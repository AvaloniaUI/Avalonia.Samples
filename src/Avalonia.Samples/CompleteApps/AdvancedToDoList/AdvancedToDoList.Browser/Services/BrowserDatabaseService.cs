using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AdvancedToDoList.Services;

namespace AdvancedToDoList.Browser.Services;

public partial class BrowserDbService : IDbService
{
    public string GetDatabasePath()
    {
        // This path must match the DATA_DIR in main.js
        return "/data/todo.db";
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