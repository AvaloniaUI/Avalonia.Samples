using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmDialogSample.ViewModels
{
    public class InteractionViewModel : ViewModelBase
    {
        public InteractionViewModel()
        {
            selectFilesInteraction = new Interaction<string?, string[]?>();
            SelectFilesCommand = ReactiveCommand.CreateFromTask(SelectFiles);
        }

        private string[]? _SelectedFiles;

        /// <summary>
        /// Gets or sets the sample text
        /// </summary>
        public string[]? SelectedFiles
        {
            get { return _SelectedFiles; }
            set { this.RaiseAndSetIfChanged(ref _SelectedFiles, value); }
        }


        private readonly Interaction<string?, string[]?> selectFilesInteraction;

        /// <summary>
        /// Gets the confirm interaction
        /// </summary>
        public Interaction<string?, string[]?> SelectFilesInteraction => this.selectFilesInteraction;


        public ICommand SelectFilesCommand { get; }

        private async Task SelectFiles()
        {
            SelectedFiles = await selectFilesInteraction.Handle("Hello from Avalonia");
        }
    }
}
