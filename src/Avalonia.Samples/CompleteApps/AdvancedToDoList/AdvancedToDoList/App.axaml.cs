using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using AdvancedToDoList.Services;
using Avalonia.Markup.Xaml;
using AdvancedToDoList.ViewModels;
using AdvancedToDoList.Views;
using System.Diagnostics.CodeAnalysis;
using AdvancedToDoList.Properties;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList;

public partial class App : Application
{
    // Static property to hold the platform-specific implementation
    public static IServiceProvider Services { get; set; } = null!;

    public static async void RegisterAppServices(IServiceCollection services)
    {
        try
        {
            // Register common services if not already registered
            if (services.All(x => x.ServiceType != typeof(ICategoryService)))
            {
                services.AddSingleton<ICategoryService, CategoryService>();
            }

            if (services.All(x => x.ServiceType != typeof(IToDoService)))
            {
                services.AddSingleton<IToDoService, ToDoService>();
            }

            Services = services.BuildServiceProvider();

            // we now have registered our services, so we can also load the settings. 
            await Settings.Default.LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());
        }
    }

    public override void Initialize()
    {
        // if we run in desgin mode, we have to register some design-mode services
        if (Design.IsDesignMode)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDbService>(new DesignDbService());
            serviceCollection.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
            serviceCollection.AddSingleton<ICategoryService, CategoryService>();
            RegisterAppServices(serviceCollection);
        }

        Settings.Default.PropertyChanged += SettingsOnPropertyChanged;
        AvaloniaXamlLoader.Load(this);
        
        UpdateAccentColor(Settings.Default.AccentColor);
    }

    // We need to update some resources if the Settings changes
    private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Settings.Default.AccentColor):
                UpdateAccentColor((sender as Settings)?.AccentColor);
                break;
        }
    }

    private void UpdateAccentColor(Color? accentColor)
    {
        var fluenttheme = Styles.OfType<FluentTheme>().FirstOrDefault();
        if (fluenttheme is null || accentColor is null)
            return;
        foreach (var palette in fluenttheme.Palettes.Values)
        {
            palette.Accent = accentColor.Value;
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "This is well tested and known to work as intended.")]
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}