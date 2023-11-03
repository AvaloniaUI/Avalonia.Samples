using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsExtensionsHostingSample.Models;
using MsExtensionsHostingSample.Services;
using MsExtensionsHostingSample.Services.Interfaces;
using MsExtensionsHostingSample.ViewModels;
using MsExtensionsHostingSample.Views;

namespace MsExtensionsHostingSample;

public partial class App : Application
{
    public IHost? GlobalHost { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var hostBuilder = CreateHostBuilder();
        var host = hostBuilder.Build();
        GlobalHost = host;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = GlobalHost.Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        DataTemplates.Add(GlobalHost.Services.GetRequiredService<ViewLocator>());
        
        base.OnFrameworkInitializationCompleted();
        
        // Usually, we don't want to block main UI thread.
        // But if it's required to start async services before we create any window,
        // then don't set any MainWindow, and simply call Show() on a new window later after async initialization. 
        await host.StartAsync();
    }
    
    private static HostApplicationBuilder CreateHostBuilder()
    {
        // Alternatively, we can use Host.CreateDefaultBuilder, but this sample focuses on HostApplicationBuilder.
        var builder = Host.CreateApplicationBuilder(Environment.GetCommandLineArgs());
        
        builder.Services.AddOptions<WeatherSettings>().Bind(builder.Configuration.GetSection("Weather"));
        builder.Services.AddHostedService<HostedBackgroundService>();

        builder.Services.AddTransient<IWeatherService, WeatherService>();
        builder.Services.AddTransient<ViewLocator>();
        builder.Services.AddTransient<MainWindowViewModel>();

        builder.Services.AddView<DayReportViewModel, DayReportView>();
        return builder;
    }

    public void NotifyStopped()
    {
        if (GlobalHost is null) return;

        GlobalHost.StopAsync().GetAwaiter().GetResult();
        GlobalHost.Dispose();
        GlobalHost = null;
    }
}