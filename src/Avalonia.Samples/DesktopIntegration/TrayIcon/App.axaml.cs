using System.Windows.Input;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using ReactiveUI;

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
            AboutCommand = ReactiveCommand.Create(ShowAboutWindow);
            ExitCommand = ReactiveCommand.Create(ExitApplication);

            DataContext = this;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _lifetime = desktop;

                desktop.ShutdownMode = Avalonia.Controls.ShutdownMode.OnExplicitShutdown;
            }

            base.OnFrameworkInitializationCompleted();
        }

        IClassicDesktopStyleApplicationLifetime? _lifetime;

        public ICommand AboutCommand { get; }
        public ICommand ExitCommand { get; }

        void ShowAboutWindow()
        {
            var window = new AboutWindow();

            window.Show();
        }

        void ExitApplication()
        {
            _lifetime?.Shutdown();
        }
    }
}

