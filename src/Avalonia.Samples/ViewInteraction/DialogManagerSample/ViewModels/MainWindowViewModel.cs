using System;
using DialogManagerSample.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DialogManagerSample.ViewModels.Shared;

namespace DialogManagerSample.ViewModels
{
    
    public partial class MainWindowViewModel : ViewModelBase, IDialogParticipant
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

        /// <summary>
        /// Gets a command that asks the user for their username. 
        /// </summary>
        /// <remarks>
        /// The current username is predefined in the input field.
        /// </remarks>
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
        
        /// <summary>
        /// Gets a context that can send notifications.
        /// </summary>
        public NotificationContext NotificationContext { get; } = new NotificationContext();

        /// <summary>
        /// Gets a command that shows an information notification.
        /// </summary>
        [RelayCommand]
        private void ShowInformation()
        {
            NotificationContext.ShowInfo("Information", "This is information.");
        }
        
        /// <summary>
        /// Gets a command that shows an error notification.
        /// </summary>
        [RelayCommand]
        private void ShowError()
        {
            NotificationContext.ShowError("Error", "Something went wrong. :-(");
        }
    }
}
