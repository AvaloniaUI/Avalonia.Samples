namespace MusicStore.ViewModels

open System.Diagnostics
open Avalonia.Media.Imaging
open Avalonia.Threading
open System
open System.Threading.Tasks
open MusicStore.Models

type AlbumViewModel(album: Album) =
    inherit ViewModelBase()
    
    let _album = album
    let mutable _cover: Bitmap | null = null
    
    member _.Artist
        with get() = _album.Artist

    member _.Title
        with get() = _album.Title
        
    member _.Cover
        with get() = _cover
        and private set value =
            if value <> _cover then
                // dispose previous bitmap to free native resources
                match _cover with
                | null -> ()
                | oldBmp -> (oldBmp :> IDisposable).Dispose()
                _cover <- value
                base.OnPropertyChanged("Cover")
        
    member this.LoadCover(?width: int) =
        task {
            try
                let width = defaultArg width 400
                use! imageStream = _album.LoadCoverBitmapAsync()
                let! bmp = Task.Run(fun () -> Bitmap.DecodeToWidth(imageStream, width))
                // Ensure property change is raised on the UI thread so the Image.Source updates correctly
                Dispatcher.UIThread.Post(fun () -> this.Cover <- bmp)
            with exn ->
                Debug.WriteLine $"Failed to load album cover: %s{exn.Message}"
        }
        
    member this.SaveToDiskAsync() =
        task {
            do! _album.SaveAsync();
    
            if not (isNull this.Cover) then
                let bitmap = this.Cover
                do! Task.Run(fun () ->
                    use fs = _album.SaveCoverBitmapStream()
                    bitmap.Save(fs)
                )
        }
