using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.MusicStore.Messages;
using Avalonia.MusicStore.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class MusicStoreViewModel : ViewModelBase
    {
        private CancellationTokenSource? _cancellationTokenSource;

        [ObservableProperty]
        public partial string? SearchText { get; set; }

        [ObservableProperty]
        public partial bool IsBusy { get; private set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(BuyMusicCommand))]
        public partial AlbumViewModel? SelectedAlbum { get; set; }

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        /// <summary>
        /// This relay command sends a message indicating that the selected album has been purchased, which will notify music store view to close.
        /// </summary>
        [RelayCommand (CanExecute = nameof(CanBuyMusic))]
        private void BuyMusic()
        {
            if (SelectedAlbum != null)
            {
                var album_exists = WeakReferenceMessenger.Default.Send(new CheckAlbumAlreadyExistsMessage(SelectedAlbum));
                if (album_exists)
                {
                    WeakReferenceMessenger.Default.Send(new NotificationMessage("This album was already added"));
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new MusicStoreClosedMessage(SelectedAlbum));
                }
            }
        }

        private bool CanBuyMusic() => SelectedAlbum != null;
        
        /// <summary>
        /// Performs an asynchronous search for albums based on the provided term and updates the results.
        /// </summary>
        private async Task DoSearch(string? term)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsBusy = true;
            SearchResults.Clear();

            var albums = await Album.SearchAsync(term);

            foreach (var album in albums)
            {
                var vm = new AlbumViewModel(album);
                SearchResults.Add(vm);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Triggered when the search text in music store view changes and initiates a new search operation.
        /// </summary>
        partial void OnSearchTextChanged(string? value)
        {
            _ = DoSearch(SearchText);
        }
    }
}
