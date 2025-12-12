using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using AdvancedToDoList;
using AdvancedToDoList.Browser.Services;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        
        System.Console.WriteLine("Begin");
        
        // Force the managed sqlite3 provider to avoid native e_sqlite3 on WASM
        try
        {
            SQLitePCL.Batteries_V2.Init();
            Console.WriteLine("SQLitePCL.Batteries_V2.Init()");
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        System.Console.WriteLine("[SQLite] Provider initialized: sqlite3");
        
        // Register the Browser service
        App.RegisterDbService(new BrowserDbService());

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}