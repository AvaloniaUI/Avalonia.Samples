using Avalonia.Markup.Xaml;
using Avalonia.BattleCity.Model;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.BattleCity
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                var mainWindow = new MainWindow();
                
                var field = new GameField();
                var game = new Game(field);
                game.Start();
                mainWindow.DataContext = field;

                lifetime.MainWindow = mainWindow;
            }
        }
    }
}

