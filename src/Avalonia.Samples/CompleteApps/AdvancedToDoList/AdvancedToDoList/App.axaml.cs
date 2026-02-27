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

/// <summary>
/// Main application entry point for the Advanced To-Do List app.
/// Handles service registration, settings management, theme updates, and UI initialization.
/// </summary>
/// <remarks>
/// Key responsibilities:
/// - Registers all application services via DI (CategoryService, ToDoItemService, DatabaseService)
/// - Manages settings persistence and live UI updates (e.g., accent color changes)
/// - Handles platform-specific UI initialization (desktop vs single-view)
/// - Configures data validation strategy (disables Avalonia's built-in validation to avoid duplicates)
/// 
/// Service registration pattern:
/// - Services are registered only if not already registered (prevents duplicates)
/// - Settings.Default is loaded after DI container is built
/// - Design mode uses a separate service collection to avoid runtime dependencies
/// 
/// Theme management:
/// - Accent color from settings is applied to all FluentTheme palettes
/// - Changes are reactive — updating settings updates the UI immediately
/// 
/// Data validation:
/// - Both Avalonia and CommunityToolkit.Mvvm have DataAnnotation validators
/// - This app uses CommunityToolkit's [NotifyDataErrorInfo], so we disable Avalonia's to avoid duplicate errors
/// </remarks>
public partial class App : Application
{
    /// <summary>
    /// Global service provider for the application.
    /// This is the primary way to access registered services from anywhere in the app.
    /// </summary>
    /// <remarks>
    /// Why static?
    /// - Allows access to services from ViewModels without constructor injection
    /// - Useful for fire-and-forget scenarios or design-time data
    /// 
    /// Initialization:
    /// - Set during RegisterAppServices() after BuildServiceProvider()
    /// - Never null after first successful initialization (enforced by ! in property declaration)
    /// </remarks>
    public static IServiceProvider Services { get; set; } = null!;

    /// <summary>
    /// Registers application services and builds the service provider.
    /// Called once during application startup.
    /// </summary>
    /// <param name="services">The collection of services to register</param>
    /// <remarks>
    /// What services are registered:
    /// - ICategoryService → CategoryService (database operations for categories)
    /// - IToDoItemService → ToDoItemService (database operations for to-do items)
    /// - IDatabaseService → DesignDbService (runtime database implementation)
    /// 
    /// Design pattern:
    /// - Checks if service already exists before adding (prevents duplicate registrations)
    /// - Uses AddSingleton for all services (all are stateless or manage their own state)
    /// 
    /// Settings loading:
    /// - Settings.Default.LoadSettingsAsync() is called AFTER DI container is built
    /// - This ensures any services needed by settings are available
    /// 
    /// Error handling:
    /// - Catches all exceptions and logs to Trace (won't crash startup)
    /// - In production, you might want more robust error handling
    /// </remarks>
    public static async void RegisterAppServices(IServiceCollection services)
    {
        try
        {
            // Register common services if not already registered
            if (services.All(x => x.ServiceType != typeof(ICategoryService)))
            {
                services.AddSingleton<ICategoryService, CategoryService>();
            }

            if (services.All(x => x.ServiceType != typeof(IToDoItemService)))
            {
                services.AddSingleton<IToDoItemService, ToDoItemService>();
            }
            
            if (services.All(x => x.ServiceType != typeof(IDatabaseService)))
            {
                services.AddSingleton<IDatabaseService, DesignDbService>();
            }

            Services = services.BuildServiceProvider();

            // We now have registered our services, so we can load the settings.
            // This allows settings to depend on any registered services if needed.
            await Settings.Default.LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            // Log to standard trace listeners (Debug window in IDE, or debugView.exe)
            Trace.TraceError(ex.ToString());
        }
    }

    /// <summary>
    /// Initializes the application resources and services.
    /// Called before OnFrameworkInitializationCompleted().
    /// </summary>
    /// <remarks>
    /// Design mode handling:
    /// - When running in VS Designer or Blend, uses DesignDbService
    /// - This provides sample data without connecting to the real database
    /// - Design mode services are registered in a temporary ServiceCollection
    /// 
    /// Settings initialization:
    /// - Subscribes to property changes for live theme updates
    /// - Loads the XAML view hierarchy (AvaloniaXamlLoader.Load)
    /// - Applies initial accent color from settings
    /// 
    /// Why Initialize() vs OnFrameworkInitializationCompleted():
    /// - Initialize() is for lightweight setup (resources, settings)
    /// - OnFrameworkInitializationCompleted() is for UI creation (MainWindow/MainView)
    /// </remarks>
    public override void Initialize()
    {
        // If we run in design mode (VS Designer, Blend), register design-time services
        if (Design.IsDesignMode)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDatabaseService>(new DesignDbService());
            serviceCollection.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());
            serviceCollection.AddSingleton<ICategoryService, CategoryService>();
            RegisterAppServices(serviceCollection);
        }

        // Subscribe to settings changes so UI updates immediately when accent color changes
        Settings.Default.PropertyChanged += SettingsOnPropertyChanged;
        
        // Load the XAML resources and views
        AvaloniaXamlLoader.Load(this);
        
        // Apply the initial accent color from settings
        UpdateAccentColor(Settings.Default.AccentColor);
    }

    /// <summary>
    /// Handles property changes from Settings to update the UI in real-time.
    /// Currently only handles accent color changes.
    /// </summary>
    /// <param name="sender">The settings object that changed</param>
    /// <param name="e">The property name that changed</param>
    /// <remarks>
    /// Why this pattern?
    /// - Keeps settings and UI in sync without user having to restart
    /// - Only updates the specific property that changed
    /// - Uses a switch statement for easy extensibility (add more properties later)
    /// </remarks>
    private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Settings.Default.AccentColor):
                UpdateAccentColor((sender as Settings)?.AccentColor);
                break;
        }
    }

    /// <summary>
    /// Updates the FluentTheme accent color across all palettes.
    /// This makes the app match the user's selected accent color.
    /// </summary>
    /// <param name="accentColor">The new accent color from settings</param>
    /// <remarks>
    /// How it works:
    /// - Finds the FluentTheme in the application styles
    /// - Updates every palette's Accent property (Primary, Secondary, etc.)
    /// - Avalonia automatically re-renders with the new color
    /// 
    /// Why check for null?
    /// - Theme might not exist yet (during startup)
    /// - Accent color might be unset (first run)
    /// - Both are handled gracefully by early returns
    /// </remarks>
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

    /// <summary>
    /// Creates the main application window and sets up the main view model.
    /// This is where the UI hierarchy is actually created.
    /// </summary>
    /// <remarks>
    /// Platform-specific handling:
    /// - IClassicDesktopStyleApplicationLifetime: Desktop apps (Windows/macOS/Linux)
    /// - ISingleViewApplicationLifetime: Mobile/Web (single-view layout)
    /// 
    /// Validation configuration:
    /// - Disables Avalonia's DataAnnotations validation plugin
    /// - Why? CommunityToolkit.Mvvm already provides [NotifyDataErrorInfo] validation
    /// - Having both would show the same error twice (bad UX)
    /// 
    /// ViewModel creation:
    /// - MainViewModel is created with default constructor (uses DI for dependencies)
    /// - Avalonia's designer mode will use parameterless constructor for preview
    /// </remarks>
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

    /// <summary>
    /// Removes Avalonia's built-in DataAnnotations validation plugin to prevent duplicate validation errors.
    /// </summary>
    /// <remarks>
    /// Why suppress trim analysis?
    /// - The BindingPlugins collection is accessed via reflection at runtime
    /// - Trim analysis can't prove these types are preserved
    /// - We know this is safe because we always use CommunityToolkit validation
    /// 
    /// Why remove these plugins?
    /// - CommunityToolkit.Mvvm provides [NotifyDataErrorInfo] and [ValidateProperty]
    /// - Avalonia's DataAnnotationsValidationPlugin does the same thing
    /// - Having both would show the same error twice (bad UX)
    /// 
    /// What's preserved?
    /// - The [NotifyDataErrorInfo] plugin from CommunityToolkit remains active
    /// - All validation logic is still fully functional
    /// </remarks>
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