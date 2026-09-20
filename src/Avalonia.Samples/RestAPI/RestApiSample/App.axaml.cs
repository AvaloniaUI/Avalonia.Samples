using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RestApiSample.Services;
using RestApiSample.ViewModels;
using RestApiSample.Views;
using System;

namespace RestApiSample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceCollection services = new ServiceCollection();
        
        services.AddHttpClient("PokeApi", client =>
        {
            client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        });

        services.AddScoped<PokeApiClient>();
        services.AddScoped<MainViewModel>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
