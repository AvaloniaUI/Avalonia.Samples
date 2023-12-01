using DialogManagerSample.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DialogManagerSample.ViewModels
{
    
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets a list of Files
        /// </summary>
        [ObservableProperty]
        private IEnumerable<string>? _SelectedFiles;
        
        /// <summary>
        /// A command used to select some files
        /// </summary>
        [RelayCommand]
        private async Task SelectFilesAsync()
        {
            SelectedFiles = await this.OpenFileDialogAsync("Hello Avalonia");
        }
    }
}
