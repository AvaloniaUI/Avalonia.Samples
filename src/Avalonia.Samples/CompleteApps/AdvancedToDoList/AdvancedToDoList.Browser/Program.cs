using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using AdvancedToDoList;
using AdvancedToDoList.Browser.Services;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        // Force the managed sqlite3 provider to avoid native e_sqlite3 on WASM
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
        SQLitePCL.raw.FreezeProvider();
        
        // Register the Browser service
        App.RegisterDbService(new BrowserDbService());

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}