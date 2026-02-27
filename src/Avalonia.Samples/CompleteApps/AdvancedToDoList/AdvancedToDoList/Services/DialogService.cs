using System.Threading.Tasks;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.Services;

/// <summary>
/// Default implementation of IDialogService for managing overlay dialogs.
/// Provides bridge between dialog participants and actual dialog framework.
/// Enables ViewModels to show dialogs without direct dependency on UI framework.
/// </summary>
/// <param name="participant">The dialog participant that will handle the actual dialog operations</param>
public class DialogService(IDialogParticipant participant) : IDialogService
{
    /// <summary>
    /// Shows an overlay dialog with the specified content and returns user response.
    /// Delegates to the dialog participant for actual dialog creation and management.
    /// Supports generic return types for flexible dialog result handling.
    /// </summary>
    /// <typeparam name="T">The type of result to return from the dialog</typeparam>
    /// <param name="title">The dialog title</param>
    /// <param name="content">The dialog content (typically a ViewModel)</param>
    /// <param name="dialogCommands">Available command buttons for the dialog</param>
    /// <returns>The dialog result of type T, or null if canceled</returns>
    public async Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands)
    {
        return await participant.ShowOverlayDialogAsync<T>(title, content, dialogCommands);
    }

    /// <summary>
    /// Returns a result from the currently active overlay dialog.
    /// Delegates to the dialog participant for proper dialog closure and result handling.
    /// Typically called from within the dialog's ViewModel to close and return data.
    /// </summary>
    /// <param name="result">The result to return from the dialog</param>
    public void ReturnResultFromOverlayDialog(object? result)
    {
        participant.ReturnResultFromOverlayDialog(result);
    }
}
