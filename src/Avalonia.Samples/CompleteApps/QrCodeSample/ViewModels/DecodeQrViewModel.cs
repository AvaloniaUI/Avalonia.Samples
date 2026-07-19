using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CodeGlyphX;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QrCodeSample.Services;

namespace QrCodeSample.ViewModels;

public partial class DecodeQrViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Gets the result of the processed QR-code if any is found in the image
    /// </summary>
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(VisitUrlCommand))]
    public partial string? Result { get; private set; }

    /// <summary>
    /// Gets the Image to decode.
    /// This is set by either the ReadImageFromClipboardAsync
    /// or ReadImageFromFileAsync commands.
    /// </summary>
    [ObservableProperty] 
    public partial Bitmap? Image { get; private set; }

    async partial void OnImageChanged(Bitmap? value)
    {
        try
        {
            if (value is null)
            {
                Result = null;
            }
            else
            {
                await ProcessQrCodeAsync();
            }
        }
        catch (Exception e)
        {
            Result = $"ERROR: {e.Message}";
        }
    }

    /// <summary>
    /// A command that tries to pull an image from the clipboard
    /// </summary>
    [RelayCommand]
    private async Task ReadImageFromClipboardAsync()
    {
        Image = await this.TryGetImageFromClipboard();
    }

    /// <summary>
    /// A command that shows a file picker dialog to select an image file and reads it into the PreviewImage property.
    /// </summary>
    [RelayCommand]
    private async Task ReadImageFromFileAsync()
    {
        var files = await this.OpenFileDialogAsync(title: "Select an image file",
            filePickerFileTypes: new[] { FilePickerFileTypes.ImageAll }, selectMany: false);

        if (files?.FirstOrDefault() is { } imageFile)
        {
            Image = new Bitmap(imageFile);
        }
    }

    /// <summary>
    /// Gets if there is a Result
    /// </summary>
    private bool HasResult => !string.IsNullOrEmpty(Result);
    
    /// <summary>
    /// A command that tries to open the Result in the default browser. 
    /// </summary>
    /// <param name="url">the URL to visit</param>
    [RelayCommand (CanExecute = nameof(HasResult))]
    private async Task VisitUrlAsync(string url)
    {
        await this.LaunchUrlAsync(url);
    }

    /// <summary>
    /// A helper method to process the image.
    /// </summary>
    private async Task ProcessQrCodeAsync()
    {
        if (Image is null)
            return;

        await Task.Run(() =>
        {
            // Save the image to a memory stream and decode it
            using var ms = new MemoryStream();
            Image.Save(ms, new PngBitmapEncoderOptions());

            var options = new QrPixelDecodeOptions()
            {
                // If this is unset and there is no valid QR-Code,
                // the processing would run forever. 
                // We will wait for 1 second, but you can configure it. 
                MaxMilliseconds = 1000 
            };
            
            
            if (QrImageDecoder.TryDecodeImage(ms, options, out var decoded))
            {
                Result = decoded.Text;
            }
            else
            {
                Result = "No QR code found in the image.";
            }
        });
    }
}