namespace MusicStore.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open CommunityToolkit.Mvvm.Messaging
open MusicStore.Messages
open System.Threading.Tasks
open MusicStore.ViewModels

type MainWindow() as this =
    inherit Window()

    let messenger =
        fun (w: MainWindow) (message: PurchaseAlbumMessage.Self) ->
            // Open the music store dialog and reply with the selected album (if any)
            let dialog = MusicStoreWindow(DataContext = MusicStoreViewModel())

            let response: Task<AlbumViewModel option> =
                task {
                    // Show the dialog modally relative to this window and await the selected album
                    let! selected = dialog.ShowDialog<AlbumViewModel>(w)
                    // If the window was closed without a selection, selected might be null
                    return
                        if obj.ReferenceEquals(selected, null) then
                            None
                        else
                            Some selected
                }

            message.Reply(response)

    do
        this.InitializeComponent()

        // Only register messenger handlers at runtime to avoid issues in the designer
        if not Design.IsDesignMode then
            // Register an AsyncRequestMessage handler that replies with a result
            WeakReferenceMessenger.Default.Register(this, messenger)

        // Kick off data loading once the window is opened and DataContext is ready
        this.Opened.Add(fun _ ->
            match this.DataContext with
            | :? MainWindowViewModel as vm -> vm.InitializeAsync() |> ignore
            | _ -> ()
        )

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)
