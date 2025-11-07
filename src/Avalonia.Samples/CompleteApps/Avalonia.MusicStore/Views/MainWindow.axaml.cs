using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
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

            // The designer cannot open a new window, so we don't register the messenger if we have a design session. 
            if (Design.IsDesignMode)
                return;

            // Whenever 'Send(new PurchaseAlbumMessage())' is called, invoke this callback on the MainWindow instance:
            WeakReferenceMessenger.Default.Register<MainWindow, PurchaseAlbumMessage>(this, static (w, m) =>
            {
                // Create an instance of MusicStoreWindow and set MusicStoreViewModel as its DataContext.
                var dialog = new MusicStoreWindow
                {
                    DataContext = new MusicStoreViewModel()
                };

                // Show dialog window and reply with returned AlbumViewModel or null when the dialog is closed.
                m.Reply(dialog.ShowDialog<AlbumViewModel?>(w));
            });
        }
    }
}
