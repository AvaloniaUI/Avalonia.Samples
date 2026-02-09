using System.Threading.Tasks;
using SharedControls.Controls;

namespace AdvancedToDoList.Services;

/// <summary>
/// Service interface for managing modal overlay dialogs.
/// Provides abstraction for dialog interactions to improve testability and platform independence.
/// Enables ViewModels to show dialogs without direct UI framework dependencies.
/// </summary>
/// <remarks>Using a service pattern improves the testability of ViewModels by allowing mock implementations.</remarks>
public interface IDialogService
{
    /// <summary>
    /// Shows a modal overlay dialog with specified content and returns user response.
    /// Typically used for confirmation dialogs, form editing, and user input scenarios.
    /// </summary>
    /// <param name="title">The dialog title displayed in the header</param>
    /// <param name="content">The dialog content (usually a ViewModel or control)</param>
    /// <param name="dialogCommands">The command buttons to display in the dialog footer</param>
    /// <typeparam name="T">The type of result expected from the dialog</typeparam>
    /// <returns>The dialog result of type T, or null if user cancels</returns>
    Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands);
    
    /// <summary>
    /// Returns a result from the currently active overlay dialog.
    /// Closes the dialog and returns the specified result to the calling code.
    /// Typically called from within the dialog's ViewModel to complete user interaction.
    /// </summary>
    /// <param name="result">The result to return from the dialog</param>
    void ReturnResultFromOverlayDialog(object? result);
}
