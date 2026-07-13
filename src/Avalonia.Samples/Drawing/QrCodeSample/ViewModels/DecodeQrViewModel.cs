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
    [ObservableProperty] public partial string? Result { get; private set; }

    [ObservableProperty] public partial Bitmap? PreviewImage { get; private set; }

    async partial void OnPreviewImageChanged(Bitmap? value)
    {
        try
        {
            await ProcessQrCodeAsync();
        }
        catch (Exception e)
        {
            Result = $"ERROR: {e.Message}";
        }
    }

    [RelayCommand]
    private async Task ReadImageFromClipboardAsync()
    {
        PreviewImage = await this.TryGetImageFromClipboard();
    }

    [RelayCommand]
    private async Task ReadImageFromFileAsync()
    {
        var files = await this.OpenFileDialogAsync(title: "Select an image file",
            filePickerFileTypes: new[] { FilePickerFileTypes.ImageAll }, selectMany: false);

        if (files?.FirstOrDefault() is { } imageFile)
        {
            PreviewImage = new Bitmap(imageFile);
        }
    }

    private async Task ProcessQrCodeAsync()
    {
        if (PreviewImage is null)
            return;

        await Task.Run(() =>
        {
            // Save the image to a memory stream and decode it
            using var ms = new MemoryStream();
            PreviewImage.Save(ms);

            var options = new QrPixelDecodeOptions()
            {
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