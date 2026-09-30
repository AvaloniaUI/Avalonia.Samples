using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CodeGlyphX;
using CodeGlyphX.Rendering;
using CodeGlyphX.Rendering.Png;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QrCodeSample.Services;

namespace QrCodeSample.ViewModels;

public partial class EncodeQrViewModel : ViewModelBase, IDialogParticipant
{
    /// <summary>
    /// Creates a new instance of the <see cref="EncodeQrViewModel"/> class and encodes the default QR-Code.
    /// </summary>
    public EncodeQrViewModel()
    {
        EncodeQr();
    }
    
    /// <summary>
    /// Gets or sets the text to create the QR-Code for. The default is our landing page.
    /// </summary>
    [ObservableProperty]
    public partial string? TextToEncode { get; set; } = "https://avaloniaui.github.io/Avalonia.Samples/";

    /// <summary>
    /// Gets or sets the foreground of the QR-Code. The default is black.
    /// </summary>
    [ObservableProperty] 
    public partial Color ForegroundColor { get; set; } = Colors.Black;
    
    /// <summary>
    /// Gets or sets the background of the QR-Code. The default is white.
    /// </summary>
    [ObservableProperty] 
    public partial Color BackgroundColor { get; set; } = Colors.White;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CopyCommand))]
    public partial Bitmap? QrCodeImage { get; private set; }

    private void EncodeQr()
    {
        if (TextToEncode == null) return;

        var qrCode = QrCodeEncoder.EncodeText(TextToEncode);

        var options = new QrEasyOptions()
        {
            Foreground = GetColorRgba32(ForegroundColor),
            Background = GetColorRgba32(BackgroundColor),
        };

        Rgba32 GetColorRgba32(Color color)
        {
            return new Rgba32(color.R, color.G, color.B, color.A);
        }

        using var ms = new MemoryStream();
        qrCode.Save(OutputFormat.Png, ms, options);

        ms.Seek(0, SeekOrigin.Begin);
        QrCodeImage = new Bitmap(ms);
    }

    private bool HasQrCodeImage => QrCodeImage != null;

    [RelayCommand(CanExecute = nameof(HasQrCodeImage))]
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

    [RelayCommand(CanExecute = nameof(HasQrCodeImage))]
    private async Task CopyAsync()
    {
        await this.TrySetImageToClipboard(QrCodeImage!);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(TextToEncode):
            case nameof(ForegroundColor):
            case nameof(BackgroundColor):
                EncodeQr();
                break;
        }
    }
}