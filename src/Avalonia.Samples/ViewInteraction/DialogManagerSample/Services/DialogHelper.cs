using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Templates;
using Avalonia.Platform.Storage;

namespace DialogManagerSample.Services;

/// <summary>
/// A helper class to manage dialogs via extension methods. Add more on your own
/// </summary>
public static class DialogHelper
{
    /// <summary>
    /// Shows an open file dialog for a registered context, most likely a ViewModel
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="title">The dialog title or a default is null</param>
    /// <param name="selectMany">Is selecting many files allowed?</param>
    /// <returns>An array of file names</returns>
    /// <exception cref="ArgumentNullException">if context was null</exception>
    public static async Task<IEnumerable<string>?> OpenFileDialogAsync(this IDialogParticipant? context,
        string? title = null,
        bool selectMany = true)
    {
        ArgumentNullException.ThrowIfNull(context);

        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        // Open the file dialog
        var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions()
            {
                AllowMultiple = selectMany,
                Title = title ?? "Select any file(s)"
            });

        // return the result
        return storageFiles.Select(s => s.Name);
    }

    /// <summary>
    /// Shows a dialog window for a given context
    /// </summary>
    /// <param name="context">The context to use</param>
    /// <param name="windowTitle">The dialog's window title</param>
    /// <param name="content">The content to show</param>
    /// <param name="contentTemplate">Optional: An <see cref="IDataTemplate"/> to represnet the <see cref="content"/></param>
    /// <typeparam name="T">The expected type to return</typeparam>
    /// <returns>The result or null if dialog was canceled</returns>
    /// <exception cref="InvalidOperationException">The dialog window can only be shown if the app is a desktop app.</exception>
    public static async Task<T?> ShowDialogWindow<T>(this IDialogParticipant? context, string windowTitle,
        object content, IDataTemplate? contentTemplate = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Get the owner window. If it is null, throw an exception
        var ownerWindow = DialogManager.GetTopLevelForContext(context) as Window
                          ?? throw new InvalidOperationException(
                              "The method ShowDialogWindow can only be used on a Window");

        // Prepare the dialog window. 
        var dialog = new Window()
        {
            Title = windowTitle,
            Content = content,
            ContentTemplate = contentTemplate,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        // Show the dialog and return it's result
        return await dialog.ShowDialog<T>(ownerWindow);
    }

    /// <summary>
    /// Closes a dialog window with the given result
    /// </summary>
    /// <param name="context">The context to resolve the window</param>
    /// <param name="result">The result to return</param>
    /// <exception cref="InvalidOperationException">If the <see cref="TopLevel"/> is not a <see cref="Window"/></exception>
    public static void ReturnResultFromDialogWindow(this IDialogParticipant? context, object? result)
    {
        ArgumentNullException.ThrowIfNull(context);

        var dialogWindow = DialogManager.GetTopLevelForContext(context) as Window
                           ?? throw new InvalidOperationException(
                               "The method ReturnResultFromDialogWindow can only be used on a Window");

        dialogWindow.Close(result);
    }

    /// <summary>
    /// Adds a notification to the <see cref="WindowNotificationManager"/>.
    /// </summary>
    /// <param name="context">The context to resolve the WindowNotificationManager</param>
    /// <param name="title">The title of the notification</param>
    /// <param name="message">The message of the notification</param>
    /// <param name="notificationType">The see <see cref="NotificationType"/> to use</param>
    /// <param name="expiration">The expiration time of the notification</param>
    /// <exception cref="InvalidOperationException">If no WindowNotificationManager was found for the given context</exception>
    public static void ShowNotificationMessage(this IDialogParticipant? context,
        string title, string message,
        NotificationType notificationType = NotificationType.Information,
        TimeSpan? expiration = null)
    {
        ShowNotificationMessage(context,
            new Notification(title, message, notificationType,
                expiration ?? TimeSpan.FromSeconds(3)));
    }

    /// <summary>
    /// Adds a given <see cref="Notification"/> to the <see cref="WindowNotificationManager"/>.
    /// </summary>
    /// <param name="context">The context to resolve the WindowNotificationManager</param>
    /// <param name="notification">The notification to display</param>
    /// <exception cref="InvalidOperationException">If no WindowNotificationManager was found for the given context</exception>
    public static void ShowNotificationMessage(this IDialogParticipant? context,
        Notification notification)
    {
        ArgumentNullException.ThrowIfNull(context);

        var notificationManager = DialogManager.GetVisualForContext(context) as WindowNotificationManager
                                  ?? throw new InvalidOperationException(
                                      "The method ShowNotificationMessage must be used on a WindowNotificationManager");

        notificationManager.Show(notification);
    }
}