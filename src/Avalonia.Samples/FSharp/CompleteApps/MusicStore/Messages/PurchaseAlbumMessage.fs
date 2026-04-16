namespace MusicStore.Messages

open MusicStore.ViewModels
open CommunityToolkit.Mvvm.Messaging.Messages

module PurchaseAlbumMessage =
    type Command = { AlbumId: int; UserId: int }
    type Response = { Success: bool; Message: string option }

    /// Message used with CommunityToolkit's messenger to request an AlbumViewModel asynchronously.
    /// Receivers should call `message.Reply(Task<AlbumViewModel option>)`.
    type Self() =
        inherit AsyncRequestMessage<AlbumViewModel option>()
