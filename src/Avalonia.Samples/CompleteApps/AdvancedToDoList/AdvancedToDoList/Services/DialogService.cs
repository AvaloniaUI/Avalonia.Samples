using System.Threading.Tasks;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.Services;

/// <summary>
/// The default implementation of the <see cref="IDialogService"/>
/// </summary>
/// <param name="participant">The <see cref="IDialogParticipant"/> to use</param>
public class DialogService(IDialogParticipant participant) : IDialogService
{
    /// <inheritdoc />
    public async Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands)
    {
        return await participant.ShowOverlayDialogAsync<T>(title, content, dialogCommands);
    }

    /// <inheritdoc />
    public void ReturnResultFromOverlayDialog(object? result)
    {
        participant.ReturnResultFromOverlayDialog(result);
    }
}
