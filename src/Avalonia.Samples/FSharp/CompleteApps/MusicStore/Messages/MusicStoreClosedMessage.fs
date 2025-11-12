namespace MusicStore.Messages

open MusicStore.ViewModels

type MusicStoreClosedMessage(selectedAlbum: AlbumViewModel) =
    member val SelectedAlbum = selectedAlbum with get
