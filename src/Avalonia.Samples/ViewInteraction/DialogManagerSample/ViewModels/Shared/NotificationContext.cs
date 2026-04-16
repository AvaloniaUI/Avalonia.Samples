using Avalonia.Controls.Notifications;
using DialogManagerSample.Services;

namespace DialogManagerSample.ViewModels.Shared;

/// <summary>
/// This class is a helper for any ViewModel to send notifications to the UI.
/// </summary>
public class NotificationContext : IDialogParticipant
{
    /// <summary>
    /// Shows a notification with information character
    /// </summary>
    /// <param name="title">the title to display</param>
    /// <param name="message">the message to display</param>
    public void ShowInfo(string title, string message)
    {
        this.ShowNotificationMessage(title, message);
    }
    
    /// <summary>
    /// Shows a notification with error character
    /// </summary>
    /// <param name="title">the title to display</param>
    /// <param name="message">the message to display</param>
    public void ShowError(string title, string message)
    {
        this.ShowNotificationMessage(title, message, NotificationType.Error);
    }
}