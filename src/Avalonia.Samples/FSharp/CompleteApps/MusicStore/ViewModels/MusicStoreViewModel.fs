namespace MusicStore.ViewModels

open System.Diagnostics
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.Windows.Input
open CommunityToolkit.Mvvm.ComponentModel
open System.Collections.ObjectModel
open CommunityToolkit.Mvvm.Input
open CommunityToolkit.Mvvm.Messaging
open MusicStore.Messages
open MusicStore.Models

type MusicStoreViewModel() as this =
    inherit ViewModelBase()

    let searchResults = ObservableCollection<AlbumViewModel>()
    let mutable _cancellationTokenSource: CancellationTokenSource option = None

    // Backing fields
    let mutable isBusy = false
    let mutable searchText = ""

    let doSearch (term: string option) =
        task {
            // set busy
            this.IsBusy <- true

            // clear results
            searchResults.Clear()

            if _cancellationTokenSource.IsSome then
                // cancel any ongoing search
                _cancellationTokenSource.Value.Cancel()
                _cancellationTokenSource.Value.Dispose()
                _cancellationTokenSource <- None

            _cancellationTokenSource <- Some(new CancellationTokenSource())

            match _cancellationTokenSource.IsNone with
            | true -> ()
            | false ->

                let cancellationToken = _cancellationTokenSource.Value.Token
                // call async search
                let! albums = Album.SearchAsync term
                albums |> Seq.map AlbumViewModel |> Seq.iter searchResults.Add
#if DEBUG
                Debug.WriteLine $"Search for '{term}' returned {searchResults.Count} results"
#endif

                // load covers
                if not cancellationToken.IsCancellationRequested then
                    do! this.LoadCovers(cancellationToken)

                // clean up cancellation token source
                if _cancellationTokenSource.IsSome then
                    _cancellationTokenSource.Value.Cancel()
                    _cancellationTokenSource.Value.Dispose()
                    _cancellationTokenSource <- None

            // unset busy
            this.IsBusy <- false
        }
        :> Task

    // ICommand backing field for Buy button (must be before members)
    let buyMusicTask =
        AsyncRelayCommand(fun () ->
            task {
                if isNull this.SelectedAlbum then
                    Debug.WriteLine "No album selected to buy"
                else
                    Debug.WriteLine $"BuyMusicCommand started for album: %A{this.SelectedAlbum.Title}"
                    WeakReferenceMessenger.Default.Send(MusicStoreClosedMessage this.SelectedAlbum) |> ignore

                Debug.WriteLine "BuyMusicCommand completed"
            }
            :> Task)

    /// Two-way bound to the search TextBox. Raises PropertyChanged on change.
    member this.SearchText
        with get () = searchText
        and set value =
            if value <> searchText then
                searchText <- value
                base.OnPropertyChanged("SearchText")
                // Trigger a new search whenever text changes
                let term =
                    match value with
                    | s when not (System.String.IsNullOrWhiteSpace s) -> Some s
                    | _ -> None

                doSearch term |> ignore

    /// Controls the visibility of the busy indicator. Raises PropertyChanged on change.
    member this.IsBusy
        with get () = isBusy
        and private set value =
            if value <> isBusy then
                isBusy <- value
                base.OnPropertyChanged("IsBusy")

    [<ObservableProperty>]
    member val SelectedAlbum: AlbumViewModel | null = null with get, set

    member this.SearchResults = searchResults

    member private this.LoadCovers(cancellationToken: CancellationToken) : Task =
        task {
            let albums = this.SearchResults.ToArray()

            let loadTasks =
                albums
                |> Array.Parallel.map (fun album ->
                    match cancellationToken.IsCancellationRequested with
                    | true -> Task.CompletedTask
                    | false -> album.LoadCover())

            let! _ = Task.WhenAny(Task.WhenAll(loadTasks), Task.Delay(Timeout.Infinite, cancellationToken))

            return ()
        }
        :> Task

    // ICommand property for Buy button
    member _.BuyMusicCommand = buyMusicTask :> ICommand
