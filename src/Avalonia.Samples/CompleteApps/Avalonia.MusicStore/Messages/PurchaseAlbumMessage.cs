using System.Collections.Generic;
using Avalonia.MusicStore.Models;
using Avalonia.MusicStore.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Avalonia.MusicStore.Messages;

/// <summary>
/// Asynchronous request message to initiate the purchase of an album.
/// Sent via the messenger to request an <see cref="AlbumViewModel"/> from a handler,
/// typically when the user wants to add a new album to their collection.
/// </summary>
public class PurchaseAlbumMessage : AsyncRequestMessage<AlbumViewModel?>
{
    public PurchaseAlbumMessage(IList<AlbumViewModel> avialableAlbums)
    {
        AvialableAlbums = avialableAlbums;
    }

    /// <summary>
    /// Gets a list of already bought albums.
    /// </summary>
    public IList<AlbumViewModel> AvialableAlbums { get; }
}
