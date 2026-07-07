using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QrCodeSample.ViewModels;

public partial class DecodeQrViewModel : ViewModelBase
{
    [ObservableProperty] 
    public partial string? Result {get; private set;}
    
    [RelayCommand]
    private async Task ReadQrCodeFromClipboardAsync()
    {
        if (App.Clipboard != null)
        {
            var bmp = await App.Clipboard.TryGetBitmapAsync();
            if (bmp is null) return;
            using var ms = new MemoryStream();

            bmp.Save(ms);

            if (QrImageDecoder.TryDecodeImage(ms, out var decoded))
            {
                Result = decoded.Text;
            }
        }
    }
}