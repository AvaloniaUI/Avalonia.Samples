using Avalonia.Data.Converters;
using Avalonia.Media;

namespace QrCodeSample.Converters;

public class BrushConverter
{
    public static IValueConverter ColorToSolidColorBrushConverter { get; } = new FuncValueConverter<Color, SolidColorBrush>(
        color => new SolidColorBrush(color)
    );
}