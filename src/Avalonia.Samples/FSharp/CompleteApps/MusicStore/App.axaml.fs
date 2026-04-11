namespace MusicStore

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open MusicStore.ViewModels
open MusicStore.Views


type App() =
    inherit Application()

    override this.Initialize() =
        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            desktop.MainWindow <- MainWindow(DataContext = MainWindowViewModel())
        | _ -> ()

        base.OnFrameworkInitializationCompleted()
