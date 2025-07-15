using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.MusicStore.Dialogs;

public static class DialogExtensions 
{
    public static async Task<T?> ShowDialogWindowAsync<T>(this IDialogParticipant context, object content,
        string? windowTitle = null)
    {
        var owner = DialogManager.GetTopLevelForContext(context) as Window;

        if  (owner == null) throw new InvalidOperationException("Unable to find Window for context");
        
        var dialogWindow = new Window()
        {
            Content = content,
            Title = windowTitle,
        };
        
        return await dialogWindow.ShowDialog<T?>(owner);
    }
    
    public static void CloseDialogWindow(this IDialogParticipant context, object? result)
    {
        var dialog = DialogManager.GetTopLevelForContext(context) as Window;
        dialog.Close(result);
    }
    
    public static async Task<MessageDialogResult> ShowMessageDialogAsync(this IDialogParticipant context, 
        string message, 
        string? title,
        MessageDialogButtons buttons = MessageDialogButtons.OkOnly)
    {
        var owner = DialogManager.GetTopLevelForContext(context) as Window;
        
        var dialogContent = new DockPanel();
        var dialog = new Window()
        {
            Content = dialogContent,
            CanResize = false,
            SizeToContent = SizeToContent.WidthAndHeight,
            Title = title,
            MinWidth = 400,
            MaxWidth = 800,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        
        var closeMessageDialogCommand = new RelayCommand<MessageDialogResult>(result => dialog.Close(result));
        
        switch (buttons)
        {
            case MessageDialogButtons.OkOnly:
                dialogContent.Children.Add(new Button()
                {
                    Content = "Ok",
                    IsDefault = true,
                    Command = closeMessageDialogCommand,
                    CommandParameter = MessageDialogResult.Ok,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    [DockPanel.DockProperty] = Dock.Bottom
                });
                break;
            case MessageDialogButtons.YesNo:
                dialogContent.Children.Add(new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 2,
                    [DockPanel.DockProperty] = Dock.Bottom,
                    Children =
                    {
                        new Button()
                        {
                            Content = "Yes",
                            IsDefault = true,
                            Command = closeMessageDialogCommand,
                            CommandParameter = MessageDialogResult.Yes
                        },
                        new Button()
                        {
                            Content = "No",
                            IsDefault = true,
                            Command = closeMessageDialogCommand,
                            CommandParameter = MessageDialogResult.No
                        }
                    }
                });
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
        }
        
        dialogContent.Children.Add(new TextBlock()
        {
            Text = message, 
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10, 10, 10, 10),
        });

        return await dialog.ShowDialog<MessageDialogResult>(owner);
    }
}