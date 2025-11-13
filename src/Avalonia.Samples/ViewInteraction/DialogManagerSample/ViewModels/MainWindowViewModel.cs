using System;
using DialogManagerSample.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace DialogManagerSample.ViewModels
{
    
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets a list of Results 
        /// </summary>
        public ObservableCollection<string> Results { get; } = ["Program started, waiting for input."];

        /// <summary>
        /// A command used to select some files
        /// </summary>
        [RelayCommand]
        private async Task SelectFilesAsync()
        {
            var results = await this.OpenFileDialogAsync("Select some files");
            
            if (results is null)
                return;
            
            foreach (var result in results)
            {
                Results.Insert(0, $"file added: {result}");
            }
        }

        [RelayCommand]
        private async Task AskForUsernameAsync()
        {
            // Setup the dialog view model
            var dialogViewModel = new InputDialogViewModel("How is your name?", Environment.UserName);
            var userName = await this.ShowDialogWindow<string?>("Question", dialogViewModel);
            
            Results.Add(
                string.IsNullOrEmpty(userName) 
                    ? "Dialog was canceled"
                    : $"The user \"{userName}\" has entered their name.");
        }
    }
}
