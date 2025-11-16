using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;

namespace TrayIcon
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public App()
        {
            DataContext = this;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        [RelayCommand]
        public void ShowAbout()
        {
            var window = new AboutWindow();

            window.Show();
        }

        [RelayCommand]
        void ExitApplication()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.TryShutdown();
            }
            else if (ApplicationLifetime is IControlledApplicationLifetime controlledLifetime)
            {
                controlledLifetime.Shutdown();
            }
        }
    }
}

