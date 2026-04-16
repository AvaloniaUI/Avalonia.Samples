using CommunityToolkit.Mvvm.Messaging.Messages;
using Avalonia.MusicStore.ViewModels;

namespace Avalonia.MusicStore.Messages
{
    public class CheckAlbumAlreadyExistsMessage : RequestMessage<bool>
    {
        public AlbumViewModel Album { get; }

        public CheckAlbumAlreadyExistsMessage(AlbumViewModel album)
        {
            Album = album;
        }
    }
}