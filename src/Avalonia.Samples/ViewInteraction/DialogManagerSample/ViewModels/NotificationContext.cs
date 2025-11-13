using Avalonia.Controls.Notifications;
using DialogManagerSample.Services;

namespace DialogManagerSample.ViewModels;

public class NotificationContext : IDialogParticipant
{
    public void ShowInfo(string title, string message)
    {
        this.ShowNotificationMessage(title, message);
    }
    
    public void ShowError(string title, string message)
    {
        this.ShowNotificationMessage(title, message, NotificationType.Error);
    }
}