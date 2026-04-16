namespace MusicStore.ViewModels

open System.Collections.ObjectModel
open System.Diagnostics
open System.Threading.Tasks
open System.Windows.Input
open CommunityToolkit.Mvvm.ComponentModel
open CommunityToolkit.Mvvm.Input
open CommunityToolkit.Mvvm.Messaging
open MusicStore.Models

type MainWindowViewModel() =
    inherit ViewModelBase()

    [<ObservableProperty>]
    member val Albums = ObservableCollection<AlbumViewModel>() with get, set

    // Called by the View once it's opened
    member this.InitializeAsync() =
        this.LoadAlbumsAsync()

    /// Expose async commands to XAML (Buttons bind to ICommand)
    member this.AddAlbumCommand =
        AsyncRelayCommand(fun () ->
            task {
                Debug.WriteLine "AddAlbumCommand started"

                // Send the message to the previously registered handler and await the selected album
                let request = MusicStore.Messages.PurchaseAlbumMessage.Self()
                WeakReferenceMessenger.Default.Send(request) |> ignore

                let! albumOpt = request.Response

                match albumOpt with
                | None -> Debug.WriteLine "AddAlbumCommand got no album"
                | Some album ->
                    Debug.WriteLine $"AddAlbumCommand got album: %A{album.Title}"
                    this.Albums.Add album
                    do! album.SaveToDiskAsync()

                Debug.WriteLine "AddAlbumCommand completed"
            }
            :> Task)
        :> ICommand

    member private this.LoadAlbumsAsync() =
        task {
            let! albums = Album.LoadCachedAsync()
            Debug.WriteLine $"Loaded %d{Seq.length albums} cached albums"
            let albumViewModels = albums |> Seq.map AlbumViewModel |> Seq.toArray

            albumViewModels |> Seq.iter (fun albumVm -> this.Albums.Add albumVm)
    
            let! _ =
                albumViewModels
                |> Seq.map _.LoadCover()
                |> Task.WhenAll
                
            return ()
        }


// NOTE: Instead of a separate AddAlbumIsRunning property (which would require raising PropertyChanged yourself),
// bind directly to AddAlbumCommand.IsRunning in XAML: {Binding AddAlbumCommand.IsRunning}
// Avalonia's binding engine can traverse the nested property path.
