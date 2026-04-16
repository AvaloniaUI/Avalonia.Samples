using System.Windows.Input;

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
        public void TrayIconClicked()
        {
            var window = new AboutWindow();

            window.SetActivationMode(ActivationMode.Click);

            window.Show();
        }

        [RelayCommand]
        public void ShowAboutWindow()
        {
            var window = new AboutWindow();

            window.SetActivationMode(ActivationMode.MenuItem);

            window.Show();
        }

        [RelayCommand]
        public void ExitApplication()
        {
            switch (ApplicationLifetime)
            {
                case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                    desktopLifetime.TryShutdown();
                    break;
                case IControlledApplicationLifetime controlledLifetime:
                    controlledLifetime.Shutdown();
                    break;
            }
        }
    }
}

