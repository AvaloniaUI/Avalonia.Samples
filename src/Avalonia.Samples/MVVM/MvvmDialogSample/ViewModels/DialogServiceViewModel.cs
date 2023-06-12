using MvvmDialogSample.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmDialogSample.ViewModels
{
    public class DialogServiceViewModel : ViewModelBase
    {
        public DialogServiceViewModel() {
            SelectFilesCommand = ReactiveCommand.CreateFromTask(SelectFilesAsync);
        }

        private IEnumerable<string>? _FileNames;

        /// <summary>
        /// Gets or sets a list of Files
        /// </summary>
        public IEnumerable<string>? FileNames
        {
            get { return _FileNames; }
            set { this.RaiseAndSetIfChanged(ref _FileNames, value); }
        }


        public ICommand SelectFilesCommand { get; }

        private async Task SelectFilesAsync()
        {
            FileNames = await this.OpenFileDialogAsync("Hello Avalonia");
        }
    }
}
