using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Desktop.Services;
using AdvancedToDoList.Services;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AdvancedToDoList.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Setup the logger
        ConfigureLogging();

        // Register the Desktop service
        var services = new ServiceCollection();
        services.AddSingleton<IDatabaseService>(new DesktopDbService());
        services.AddSingleton<ISettingsStorageService>(new DefaultSettingsStorageService());

        App.RegisterAppServices(services);

        try
        {
            // Just a hint for us to see if the App is AOT compiled
            Log.Information("Starting Avalonia app (AOT={AOT})",
                System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled == false);
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly during startup");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void ConfigureLogging()
    {
        try
        {
            // Establish the base directory where the app is running (required for logging paths)
            var baseDir = AppContext.BaseDirectory;
            // Create a dedicated 'logs' subdirectory for app logs
            var logDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logDir);
            // Define log filename pattern (Serilog will append date for rolling)
            var logPath = Path.Combine(logDir, "app-.log");

            // Build Serilog configuration
            var cfg = new LoggerConfiguration()
#if DEBUG
                    // More verbose logging in debug builds
                    .MinimumLevel.Debug()
#else
                    // Less verbose in release builds
                    .MinimumLevel.Information()
#endif
                    // Write logs to files with daily rotation
                    .WriteTo.File(
                        logPath,
                        rollingInterval: RollingInterval.Day, // Create new log file each day
                        retainedFileCountLimit: 7, // Keep only 7 days of logs
                        shared: true, // Allow multiple instances to write
                        flushToDiskInterval: TimeSpan.FromSeconds(1), // Periodically flush to disk
                        buffered: false, // Write directly for reliability
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                ;

            // Install the configured logger as the global Serilog logger
            Log.Logger = cfg.CreateLogger();
            // Route Avalonia's internal Trace output through Serilog for unified logs
            Trace.Listeners.Add(new SerilogTraceListener.SerilogTraceListener());

            // Add global exception handlers to ensure uncaught errors are logged
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                Log.Fatal(e.ExceptionObject as Exception, "[FATAL] Unhandled exception in AppDomain");

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                Log.Error(e.Exception, "[ERROR] Unobserved task exception");
                e.SetObserved(); // Prevents finalizer from re-raising the exception
            };
        }
        catch
        {
            // Last resort: silently fail to avoid crashing the app if logging setup fails (e.g., under AOT)
        }
    }
}