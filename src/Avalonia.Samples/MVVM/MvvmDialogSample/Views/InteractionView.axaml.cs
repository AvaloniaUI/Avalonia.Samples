using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using MvvmDialogSample.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace MvvmDialogSample.Views
{
    public partial class InteractionView : ReactiveUserControl<InteractionSample>
    {
        public InteractionView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(ViewModel.Confirm.RegisterHandler(this.InteractionHandler));
            });
        }

        private async Task InteractionHandler(InteractionContext<string?, string?> context)
        {
            // Get our parent top level control in order to get the needed service (in our sample the storage provider. Can also be the clipboard etc.)
            var topLevel = TopLevel.GetTopLevel(this);

            var storageFile = await topLevel!.StorageProvider
                .OpenFilePickerAsync(
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
