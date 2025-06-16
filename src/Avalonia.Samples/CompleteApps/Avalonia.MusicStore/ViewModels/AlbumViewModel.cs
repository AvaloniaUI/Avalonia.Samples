using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.MusicStore.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class AlbumViewModel : ViewModelBase
    {
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
                await using (var imageStream = await _album.LoadCoverBitmapAsync())
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
            await _album.SaveAsync();

            if (await LoadCoverAsync() is Bitmap cover)
            {
                await Task.Run(() =>
                {
                    using (var fs = _album.SaveCoverBitmapStream())
                    {
                        cover.Save(fs);
                    }
                });
            }
        }
    }
}
