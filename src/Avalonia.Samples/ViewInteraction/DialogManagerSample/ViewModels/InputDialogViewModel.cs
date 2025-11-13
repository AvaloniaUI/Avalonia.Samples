using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogManagerSample.Services;

namespace DialogManagerSample.ViewModels;

public partial class InputDialogViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Creates a new input dialog view model 
    /// </summary>
    /// <param name="prompt">The prompt text to use</param>
    /// <param name="defaultValue">the default value to display</param>
    public InputDialogViewModel(string prompt = "Input text:", string? defaultValue = null)
    {
        PromptText = prompt;
        InputText = defaultValue;
    }
    
    /// <summary>
    /// Gets or sets the prompt text to display
    /// </summary>
    [ObservableProperty]
    public partial string PromptText { get; set; }
    
    /// <summary>
    /// Gets or sets the text that the user has entered
    /// </summary>
    [ObservableProperty]
    [Required]
    [NotifyCanExecuteChangedFor(nameof(ReturnResultCommand))]
    public partial string? InputText { get; set; }

    /// <summary>
    /// Gets a command to return the result
    /// </summary>
    [RelayCommand (CanExecute = nameof(CanReturnResult))]
    private void ReturnResult()
    {
        this.ReturnResultFromDialogWindow(InputText);
    }

    private bool CanReturnResult() => !string.IsNullOrEmpty(InputText);
    
    /// <summary>
    /// Gets a command to cancel the input
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        this.ReturnResultFromDialogWindow(null);
    }
}