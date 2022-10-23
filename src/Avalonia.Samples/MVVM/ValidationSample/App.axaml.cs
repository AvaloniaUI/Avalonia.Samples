using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ValidationSample.ViewModels;
using ValidationSample.Views;

namespace ValidationSample
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new ValidationUsingExceptionInsideSetterViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
