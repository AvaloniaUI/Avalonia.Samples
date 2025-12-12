using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using AdvancedToDoList.Android.Services;

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
        App.RegisterDbService(new AndroidDbService());

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
