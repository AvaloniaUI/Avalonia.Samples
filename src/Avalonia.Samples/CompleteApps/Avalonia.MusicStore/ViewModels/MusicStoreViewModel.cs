using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.MusicStore.Dialogs;
using Avalonia.MusicStore.Messages;
using Avalonia.MusicStore.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class MusicStoreViewModel : ViewModelBase, IDialogParticipant
    {
        private IList<AlbumViewModel> _availableAlbums;
        private CancellationTokenSource? _cancellationTokenSource;

        public MusicStoreViewModel(IList<AlbumViewModel> availableAlbums)
        {
            _availableAlbums = availableAlbums;
        }

        [ObservableProperty] public partial string? SearchText { get; set; }

        [ObservableProperty] public partial bool IsBusy { get; private set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(BuyMusicCommand))]
        public partial AlbumViewModel? SelectedAlbum { get; set; }

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        /// <summary>
        /// This relay command sends a message indicating that the selected album has been purchased, which will notify music store view to close.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanBuyMusic))]
        private async Task BuyMusicAsync()
        {
            if (SelectedAlbum == null)
            {
                throw new NullReferenceException("SelectedAlbum is null");
            }
            else if (_availableAlbums.Contains(SelectedAlbum))
            {
                await this.ShowMessageDialogAsync("Already bought this album", "Info");
            }
            else
            {
                this.CloseDialogWindow(SelectedAlbum);
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