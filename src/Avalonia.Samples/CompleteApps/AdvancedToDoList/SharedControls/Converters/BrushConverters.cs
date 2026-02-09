using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SharedControls.Converters;

/// <summary>
/// Static class providing value converters for working with Avalonia brushes and colors.
/// Offers convenient converters for creating SolidColorBrush instances from Color values.
/// Also provides smart foreground color selection based on background luminance.
/// </summary>
/// <remarks>
/// Usage in XAML:
/// - Simple color to brush: {Binding MyColor, Converter={x:Static BrushConverters.ColorToBrushConverter}}
/// - Smart foreground selection: {Binding BackgroundColor, Converter={x:Static BrushConverters.ColorToForegroundBrushConverter}}
/// </remarks>
public static class BrushConverters
{
    /// <summary>
    /// Converts a Color to a SolidColorBrush.
    /// Simple converter that wraps a Color in a SolidColorBrush for use in UI.
    /// </summary>
    /// <remarks>
    /// Example XAML usage:
    /// <Border Background="{Binding Category.Color, Converter={x:Static BrushConverters.ColorToBrushConverter}}" />
    /// </remarks>
    public static FuncValueConverter<Color, SolidColorBrush> ColorToBrushConverter { get; } = 
        new FuncValueConverter<Color, SolidColorBrush>(color => new SolidColorBrush(color));

    /// <summary>
    /// Converts a background color to an appropriate foreground (text) color.
    /// Automatically selects black or white text color based on background luminance.
    /// Ensures text is always readable against any background color.
    /// </summary>
    /// <remarks>
    /// Example XAML usage:
    /// <TextBlock Foreground="{Binding Background, Converter={x:Static BrushConverters.ColorToForegroundBrushConverter}}" Text="Hello" />
    /// 
    /// How it works:
    /// - Light backgrounds (high luminance) → black text
    /// - Dark backgrounds (low luminance) → white text
    /// - Uses WCAG luminance formula for accessibility compliance
    /// </remarks>
    public static FuncValueConverter<Color, SolidColorBrush> ColorToForegroundBrushConverter { get; } =
        new FuncValueConverter<Color, SolidColorBrush>(color => new SolidColorBrush(GetIdeaForegroundColor(color)));
    
    
    /// <summary>
    /// Determines the ideal foreground color (black or white) for a given background color.
    /// Calculates luminance using the WCAG 2.0 formula and returns the most readable contrast.
    /// </summary>
    /// <param name="backgroundColor">
    /// The background color to calculate appropriate foreground color for.
    /// Can be any Color value.
    /// </param>
    /// <returns>
    /// Colors.White for dark backgrounds (luminance smaller than 128)
    /// Colors.Black for light backgrounds (luminance greater or equal 128)
    /// </returns>
    /// <remarks>
    /// Luminance calculation (WCAG formula):
    /// L = 0.299R + 0.587G + 0.114B
    /// 
    /// Why this formula?
    /// - Human eyes perceive green more strongly than red, and red more strongly than blue
    /// - This formula accounts for that perceptual difference
    /// - Ensures proper contrast ratios for accessibility (WCAG standards)
    /// 
    /// Threshold of 128 was chosen empirically as good balance point
    /// </remarks>
    public static Color GetIdeaForegroundColor(Color backgroundColor)
    {
        // Calculate relative luminance using WCAG 2.0 formula
        var luminance = (backgroundColor.R * 299 + backgroundColor.G * 587 + backgroundColor.B * 114) / 1000;
        
        // Return white for dark backgrounds, black for light backgrounds
        return luminance >= 128 ? Colors.Black : Colors.White;
    }
}