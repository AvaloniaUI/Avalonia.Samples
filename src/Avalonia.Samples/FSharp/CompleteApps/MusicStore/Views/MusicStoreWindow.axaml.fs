namespace MusicStore.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open CommunityToolkit.Mvvm.Messaging
open MusicStore.Messages

type MusicStoreWindow() as this =
    inherit Window ()
    
    let onMusicStoreClosed (w: MusicStoreWindow) (msg: MusicStoreClosedMessage) =
        // Handle the music store closed message, e.g., retrieve the selected album
        let selectedAlbum = msg.SelectedAlbum
        // Perform actions with the selected album as needed
        // For example, you might want to display its details or add it to a collection
        w.Close selectedAlbum

    do 
        this.InitializeComponent()
        // Ensure the window and its child views have a proper DataContext at runtime
        // this.DataContext <- MusicStoreViewModel()
        WeakReferenceMessenger.Default.Register(this, onMusicStoreClosed)

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)
