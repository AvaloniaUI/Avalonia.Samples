using System;
using Avalonia.Controls;
using Avalonia.MusicStore.Messages;
using Avalonia.Controls.Notifications;
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
            
            WeakReferenceMessenger.Default.Register<MusicStoreWindow, NotificationMessage>(this, static (w, m) =>
            {
                w.NotificationManager.CloseAll();
                w.NotificationManager.Show(m.Message, NotificationType.Warning, TimeSpan.FromSeconds(3));
            });
        }
    }
}
