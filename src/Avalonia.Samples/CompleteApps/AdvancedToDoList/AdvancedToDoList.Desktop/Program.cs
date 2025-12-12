using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Desktop.Services;
using Avalonia;
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
        App.RegisterDbService(new DesktopDbService());
        
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
            var baseDir = AppContext.BaseDirectory;
            var logDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logDir);
            var logPath = Path.Combine(logDir, "app-.log");

            var cfg = new LoggerConfiguration()
#if DEBUG
                    .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                    .WriteTo.File(
                        logPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                        buffered: false,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                ;

            Log.Logger = cfg.CreateLogger();
            // Route Avalonia Trace logs into Serilog
            Trace.Listeners.Add(new SerilogTraceListener.SerilogTraceListener());

            // Listen to unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Log.Fatal(e.ExceptionObject as Exception, "[FATAL] Unhandled exception in AppDomain");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Error(e.Exception, "[ERROR] Unobserved task exception");
                e.SetObserved();
            };
        }
        catch
        {
            // Last resort: avoid crashing if logging setup fails under AOT
        }
    }
}
