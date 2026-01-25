using System.Threading.Tasks;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.Services;

/// <summary>
/// This is a service that interacts with the <see cref="DialogManager"/>. 
/// </summary>
/// <remarks>Using this a service improves the testability of our App.</remarks>
public interface IDialogService
{
    /// <summary>
    /// This method shows an overlay dialog and returns its result.
    /// </summary>
    /// <param name="title">The dialog title</param>
    /// <param name="content">The content to show</param>
    /// <param name="dialogCommands">The <see cref="DialogCommands"/> to show</param>
    /// <typeparam name="T">The expected return type</typeparam>
    /// <returns></returns>
    Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands);
    
    /// <summary>
    /// This returns the result from an overlay dialog.
    /// </summary>
    /// <param name="result">The result to return</param>
    void ReturnResultFromOverlayDialog(object? result);
}
