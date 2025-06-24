using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Avalonia.MusicStore.Messages;

public class NotificationMessage 
{
    public NotificationMessage(string message)
     {
         Message = message;
     }
    public string Message { get; }
}