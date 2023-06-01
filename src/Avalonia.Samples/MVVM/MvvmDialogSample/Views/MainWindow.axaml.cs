using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MvvmDialogSample.ViewModels;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace MvvmDialogSample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        IDisposable? filePickerInteractionDisposable;

        protected override void OnDataContextChanged(EventArgs e)
        {
            filePickerInteractionDisposable?.Dispose();

            if (DataContext is MainWindowViewModel vm)
            {
                filePickerInteractionDisposable = 
                    vm.InteractionSample.Confirm.RegisterHandler(InteractionHandler);
            }

            base.OnDataContextChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            filePickerInteractionDisposable?.Dispose();
            base.OnClosed(e);
        }

        private async Task InteractionHandler(InteractionContext<string?, string?> context)
        {
                var storageFile = await StorageProvider.OpenFilePickerAsync(
                                new FilePickerOpenOptions()
                                {
                                    AllowMultiple = false,
                                    Title = "Select any file(s)"
                                });

                if (storageFile != null)
                {
                    context.SetOutput(storageFile[0].Name);
                }
            
        }
    }
}