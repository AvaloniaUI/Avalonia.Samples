using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace QrCodeSample.Services;

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
    /// <param name="filePickerFileTypes">Filetypes to filter for. The default is <see cref="FilePickerFileTypes.All"/></param>
    /// <param name="selectMany">Is selecting many files allowed?</param>
    /// <returns>An array of file names</returns>
    /// <exception cref="ArgumentNullException">if context was null</exception>
    public static async Task<IEnumerable<string>?> OpenFileDialogAsync(this IDialogParticipant? context,
        string? title = null,
        FilePickerFileType[]? filePickerFileTypes = null,
        bool selectMany = false)
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
                FileTypeFilter = filePickerFileTypes ?? [FilePickerFileTypes.All],
                Title = title ?? "Select any file(s)"
            });

        // return the result as full local paths where available, falling back to the file name
        return storageFiles.Select(s => s.TryGetLocalPath() ?? s.Name);
    }
    
    public static async Task<SaveFilePickerResult?> SaveFileDialogAsync(this IDialogParticipant? context,
        string? title = null,
        FilePickerFileType[]? filePickerFileTypes = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        // Open the file dialog
        var result = await topLevel.StorageProvider.SaveFilePickerWithResultAsync(
            new FilePickerSaveOptions()
            {
                FileTypeChoices = filePickerFileTypes ?? [FilePickerFileTypes.All],
                Title = title ?? "Select any file(s)"
            });

        // return the result as full local paths where available, falling back to the file name
        return result;
    }

    public static async Task<Bitmap?> TryGetImageFromClipboard(this IDialogParticipant? context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        if (topLevel.Clipboard != null)
        {
            var result = await topLevel.Clipboard.TryGetBitmapAsync();
            return result;
        }
        return null;
    }   
    
    public static async Task<bool?> TrySetImageToClipboard(this IDialogParticipant? context, Bitmap? bitmap)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (bitmap == null) return false;
        
        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        if (topLevel.Clipboard != null)
        {
            await topLevel.Clipboard.SetBitmapAsync(bitmap);
            return true;
        }
        return false;
    }
}