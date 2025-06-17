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

        [ObservableProperty] public partial Bitmap? Cover { get; private set; }

        /// <summary>
        /// Asynchronously loads and decodes the album cover image, then assigns it to <see cref="Cover"/>.
        /// </summary>
        public async Task LoadCover()
        {
            await using (var imageStream = await _album.LoadCoverBitmapAsync())
            {
                Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
        }

        /// <summary>
        /// Saves the album and its cover to cache.
        /// </summary>        
        public async Task SaveToDiskAsync()
        {
            await _album.SaveAsync();

            if (Cover != null)
            {
                var bitmap = Cover;

                await Task.Run(() =>
                {
                    using (var fs = _album.SaveCoverBitmapStream())
                    {
                        bitmap.Save(fs);
                    }
                });
            }
        }
    }
}
