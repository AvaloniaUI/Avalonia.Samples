using Avalonia.Controls;
using Avalonia.MusicStore.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace Avalonia.MusicStore.Views
{
    public partial class MusicStoreWindow : Window
    {
        public MusicStoreWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<MusicStoreWindow, MusicStoreClosedMessage>(this,
                static (w, m) => w.Close(m.SelectedAlbum));
        }
    }
}
