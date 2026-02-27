using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using AdvancedToDoList.Android.Services;
using AdvancedToDoList.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.Android;

[Activity(
    Label = "AdvancedToDoList.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        // Register the Android service
        var services = new ServiceCollection();
        services.AddSingleton<IDatabaseService>(new AndroidDbService());
        services.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
        
        App.RegisterAppServices(services);

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
