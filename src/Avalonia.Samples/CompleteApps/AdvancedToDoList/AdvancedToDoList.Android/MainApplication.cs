using System;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using AdvancedToDoList.Android.Services;
using AdvancedToDoList.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.Android;

[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
    protected MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        // Register Android-specific services before App initialization.
        var services = new ServiceCollection();
        services.AddSingleton<IDatabaseService>(new AndroidDbService());
        services.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());

        App.RegisterAppServices(services);

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}

