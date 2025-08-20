using Avalonia.Controls;
using Avalonia.MusicStore.Messages;
using Avalonia.MusicStore.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.MusicStore.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Design.IsDesignMode)
                return;

            WeakReferenceMessenger.Default.Register<MainWindow, PurchaseAlbumMessage>(this, static (w, m) =>
            {
                var dialog = new MusicStoreWindow
                {
                    DataContext = new MusicStoreViewModel()
                };

                m.Reply(dialog.ShowDialog<AlbumViewModel?>(w));
            });
        }
    }
}
