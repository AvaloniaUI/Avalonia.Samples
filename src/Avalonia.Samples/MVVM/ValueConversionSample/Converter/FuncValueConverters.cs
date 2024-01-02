using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ValueConversionSample.Converter;

/// <summary>
/// A static class holding our FuncValueConverter
/// </summary>
/// <remarks>
/// Consume it from XAML via <code>{x:Static conv:FuncValueConverters.MyConverter}</code>
/// </remarks>
public static class FuncValueConverters
{
    /// <summary>
    /// Gets a Converter that returns a parsed Brush for a given input. Returns null if the input was not parsed successfully
    /// </summary>
    public static FuncValueConverter<string?, Brush?> StringToBrushConverter { get; } = 
        new FuncValueConverter<string?, Brush?>(s =>
        {
            // define output variable
            Color color;
            
            // try parse color. If that was not successful try to add a leading '#'
            if (Color.TryParse(s, out color) || Color.TryParse($"#{s}", out color))
            {
                return new SolidColorBrush(color);
            }
            
            // If string was not convertible, we return null
            return null;
        });
}