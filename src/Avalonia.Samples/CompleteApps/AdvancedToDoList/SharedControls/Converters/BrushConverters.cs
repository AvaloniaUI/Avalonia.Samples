using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SharedControls.Converters;

public static class BrushConverters
{
    public static FuncValueConverter<Color, SolidColorBrush> ColorToBrushConverter { get; } = 
        new FuncValueConverter<Color, SolidColorBrush>(color => new SolidColorBrush(color));

    public static FuncValueConverter<Color, SolidColorBrush> ColorToForegroundBrushConverter { get; } =
        new FuncValueConverter<Color, SolidColorBrush>(color => new SolidColorBrush(GetIdeaForegroundColor(color)));

    
    public static Color GetIdeaForegroundColor(Color backgroundColor)
    {
        var luminance = (backgroundColor.R * 299 + backgroundColor.G * 587 + backgroundColor.B * 114) / 1000;
        return luminance >= 128 ? Colors.Black : Colors.White;
    }
}