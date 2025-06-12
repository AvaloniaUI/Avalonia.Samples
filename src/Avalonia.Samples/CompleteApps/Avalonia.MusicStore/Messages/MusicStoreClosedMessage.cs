using Avalonia.MusicStore.ViewModels;

namespace Avalonia.MusicStore.Messages;

/// <summary>
/// Message indicating that the music store has been closed after a purchase.
/// Carries the <see cref="SelectedAlbum"/> that was purchased,
/// allowing other components to update their state accordingly.
/// </summary>
public class MusicStoreClosedMessage(AlbumViewModel selectedAlbum)
{
    public AlbumViewModel SelectedAlbum { get; } = selectedAlbum;
}
