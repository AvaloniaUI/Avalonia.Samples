using System.Threading.Tasks;
using SharedControls.Controls;

namespace AdvancedToDoList.Services;

public interface IDialogService
{
    Task<T?> ShowOverlayDialogAsync<T>(string title, object? content, params DialogCommand[] dialogCommands);
    
    void ReturnResultFromOverlayDialog(object? result);
}
