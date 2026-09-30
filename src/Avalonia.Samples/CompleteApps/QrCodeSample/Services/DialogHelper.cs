using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    /// <summary>
    /// Tries to launch a provided URL string. 
    /// </summary>
    /// <param name="context">the context to resolve the view for</param>
    /// <param name="url">the url to launch</param>
    /// <returns>true if successful, otherwise false</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<bool> LaunchUrlAsync(this IDialogParticipant context, string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        var topLevel = DialogManager.GetTopLevelForContext(context);

        try
        {
            // Try to create the Uri as supplied.
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            // If it’s relative (e.g. "t.me"), turn it into an absolute web Uri.
            if (!uri.IsAbsoluteUri)
            {
                // Prepend a default scheme – you can change this to any fallback you prefer.
                uri = new Uri("https://" + url.TrimStart('/'), UriKind.Absolute);
            }

            // Launch the (now‑absolute) Uri.
            return await topLevel!.Launcher.LaunchUriAsync(uri);
        }
        catch (UriFormatException)
        {
            // The string couldn’t be turned into a valid Uri at all.
            return false;
        }
        catch (Exception e)
        {
            // Any other unexpected error (e.g., launcher failure).
            Trace.WriteLine($"Failed to launch URL '{url}':\n {e.Message}");
            return false;
        }
    }
}