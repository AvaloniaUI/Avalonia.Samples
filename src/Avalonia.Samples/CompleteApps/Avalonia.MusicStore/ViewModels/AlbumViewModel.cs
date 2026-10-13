using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.MusicStore.Models;
using Avalonia.MusicStore.Services;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class AlbumViewModel : ViewModelBase, IEquatable<AlbumViewModel>
    {
        private static readonly AlbumService s_albumService = new();
        private readonly Album _album;

        public AlbumViewModel(Album album)
        {
            _album = album;
        }

        public string Artist => _album.Artist;

        public string Title => _album.Title;

        public Task<Bitmap?> Cover => LoadCoverAsync();

        /// <summary>
        /// Asynchronously loads and decodes the album cover image, then assigns it to <see cref="Cover"/>.
        /// </summary>
        private async Task<Bitmap?> LoadCoverAsync()
        {
            try
            {
                // We wait a few ms to demonstrate that the images are loaded in the background. 
                // Remove this line in production.
                await Task.Delay(200);
                
                await using (var imageStream = await s_albumService.LoadCoverBitmapAsync(_album))
                {
                    return await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the album and its cover to cache.
        /// </summary>        
        public async Task SaveToDiskAsync()
        {
            await s_albumService.SaveAsync(_album);

            if (await LoadCoverAsync() is { } cover)
            {
                await Task.Run(() =>
                {
                    using (var fs = s_albumService.SaveCoverBitmapStream(_album))
                    {
                        cover.Save(fs);
                    }
                });
            }
        }

        public bool Equals(AlbumViewModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _album.Equals(other._album);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AlbumViewModel)obj);
        }

        public override int GetHashCode()
        {
            return _album.GetHashCode();
        }
    }
}
