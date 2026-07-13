using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CodeGlyphX;
using CodeGlyphX.Rendering;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QrCodeSample.Services;

namespace QrCodeSample.ViewModels;

public partial class EncodeQrViewModel : ViewModelBase, IDialogParticipant
{
    [ObservableProperty]
    public partial string? TextToEncode { get; set; }
    
    [ObservableProperty]
    public partial Color ForegroundColor {get;set;}
    
    [ObservableProperty]
    public partial Color BackgroundColor {get;set;}
    
    partial void OnTextToEncodeChanged(string? value)
    {
        EncodeQr();
    }
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopyCommand))]
    public partial Bitmap? QrCodeImage { get; private set; }

    private void EncodeQr()
    {
        if (TextToEncode == null) return;
        
        var qrCode = QrCodeEncoder.EncodeText(TextToEncode);

        using var ms = new MemoryStream();
        qrCode.Save(OutputFormat.Png, ms);
        
        ms.Seek(0, SeekOrigin.Begin);
        QrCodeImage = new Bitmap(ms);
    }

    private bool HasQrCodeImage => QrCodeImage != null;
    
    [RelayCommand (CanExecute = nameof(HasQrCodeImage))]
    private async Task SaveAsync()
    {
        var saveFile = await this.SaveFileDialogAsync(title: "Save QR Code Image", 
            filePickerFileTypes: new[] { FilePickerFileTypes.ImagePng });

        if (saveFile.HasValue && QrCodeImage is not null)
        {
            await using var stream = await saveFile.Value.File!.OpenWriteAsync();
            QrCodeImage.Save(stream);
        }
    }

    [RelayCommand (CanExecute = nameof(HasQrCodeImage))]
    private async Task CopyAsync()
    {
        await this.TrySetImageToClipboard(QrCodeImage!);
    }
}