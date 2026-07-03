using AdvancedToDoList.Android.Services;
using AdvancedToDoList.Services;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.Android
{
    [Application]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override AppBuilder CreateAppBuilder()
        {
            // Register the Android service
            var services = new ServiceCollection();
            services.AddSingleton<IDatabaseService>(new AndroidDbService());
            services.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());

            App.RegisterAppServices(services);
            
            return base.CreateAppBuilder()
                .WithInterFont();
        }
    }
}
