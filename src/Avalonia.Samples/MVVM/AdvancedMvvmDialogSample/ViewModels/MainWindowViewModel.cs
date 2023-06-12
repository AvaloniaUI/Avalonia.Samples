using AdvancedMvvmDialogSample.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdvancedMvvmDialogSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SelectFilesCommand = ReactiveCommand.CreateFromTask(SelectFilesAsync);
        }

        private IEnumerable<string>? _SelectedFiles;

        /// <summary>
        /// Gets or sets a list of Files
        /// </summary>
        public IEnumerable<string>? SelectedFiles
        {
            get { return _SelectedFiles; }
            set { this.RaiseAndSetIfChanged(ref _SelectedFiles, value); }
        }


        public ICommand SelectFilesCommand { get; }

        private async Task SelectFilesAsync()
        {
            SelectedFiles = await this.OpenFileDialogAsync("Hello Avalonia");
        }
    }
}
