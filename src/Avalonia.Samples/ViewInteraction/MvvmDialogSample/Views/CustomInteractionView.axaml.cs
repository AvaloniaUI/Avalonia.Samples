using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using MvvmDialogSample.ViewModels;
using System.Linq;

namespace MvvmDialogSample.Views
{
    public partial class CustomInteractionView : UserControl
    {
        public CustomInteractionView()
        {
            InitializeComponent();
        }

        // Stores a reference to the disposable in order to clean it up if needed
        IDisposable? _selectFilesInteractionDisposable;

        protected override void OnDataContextChanged(EventArgs e)
        {
            // Dispose any old handler
            _selectFilesInteractionDisposable?.Dispose();

            if (DataContext is CustomInteractionViewModel vm)
            {
                // register the interaction handler
                _selectFilesInteractionDisposable =
                    vm.SelectFilesInteraction.RegisterHandler(InteractionHandler);
            }

            base.OnDataContextChanged(e);
        }

        private async Task<string[]?> InteractionHandler(string input)
        {
            // Get a reference to our TopLevel (in our case the parent Window)
            var topLevel = TopLevel.GetTopLevel(this);

            // Try to get the files
            var storageFiles = await topLevel!.StorageProvider.OpenFilePickerAsync(
                            new FilePickerOpenOptions() 
                            { 
                                AllowMultiple = true, 
                                Title = input
                            });

            // Transform the files as needed and return them. If no file was selected, null will be returned
            return storageFiles?.Select(x => x.Name)?.ToArray();
        }
    }
}
