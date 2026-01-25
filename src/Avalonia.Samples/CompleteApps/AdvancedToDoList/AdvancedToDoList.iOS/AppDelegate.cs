using Foundation;
using UIKit;
using Avalonia;
using Avalonia.Controls;
using Avalonia.iOS;
using Avalonia.Media;
using AdvancedToDoList.iOS.Services;
using AdvancedToDoList.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public partial class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        // Register the iOS service
        var services = new ServiceCollection();
        services.AddSingleton<IDatabaseService>(new IosDatabaseService());
        services.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
        
        App.RegisterAppServices(services);
        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
