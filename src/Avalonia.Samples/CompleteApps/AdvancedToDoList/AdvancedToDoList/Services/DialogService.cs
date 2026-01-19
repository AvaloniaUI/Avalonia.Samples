using System.Threading.Tasks;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.Services;

public class DialogService(IDialogParticipant participant) : IDialogService
{
    public async Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands)
    {
        return await participant.ShowOverlayDialogAsync<T>(title, content, dialogCommands);
    }

    public void ReturnResultFromOverlayDialog(object? result)
    {
        participant.ReturnResultFromOverlayDialog(result);
    }
}
