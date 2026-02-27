using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

/// <summary>
/// Value converter that creates rounded corners based on control bounds.
/// Automatically calculates corner radius to make controls appear circular/rounded.
/// Radius is always half of the smaller dimension (width or height).
/// </summary>
/// <remarks>
/// Common use cases:
/// - Creating circular buttons from rectangular bounds
/// - Making panels with rounded corners that match their size
/// - Creating avatar circles from square image containers
/// 
/// Example XAML usage:
/// <Ellipse CornerRadius="{Binding Bounds, Converter={x:Static BoundsToCornerRadiusConverter.Instance}}" />
/// </remarks>
public class BoundsToCornerRadiusConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance of this converter for use in XAML bindings.
    /// Use this static instance instead of creating new ones for better performance.
    /// </summary>
    public static BoundsToCornerRadiusConverter Instance { get; } = new();
    
    /// <summary>
    /// Converts control bounds to appropriate corner radius.
    /// Calculates radius as half of the smaller dimension to ensure perfect circles.
    /// </summary>
    /// <param name="value">
    /// The control bounds (Rect) containing width and height information.
    /// Usually comes from LayoutInformation.GetLayoutSlot() or similar.
    /// </param>
    /// <param name="targetType">
    /// The type of binding target property (should be CornerRadius).
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <param name="parameter">
    /// Optional parameters (not used by this converter).
    /// </param>
    /// <param name="culture">
    /// Culture information for localization.
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <returns>
    /// CornerRadius with value equal to half of the smaller dimension,
    /// or DoNothing if value is not a Rect.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Rect bounds)
        {
            // Use the smaller dimension to ensure the shape fits within bounds
            var minSize = Math.Min(bounds.Width, bounds.Height);
            
            // Radius of circle = diameter / 2
            return new CornerRadius(minSize / 2);
        }

        // Don't apply corner radius if we don't have valid bounds
        return BindingOperations.DoNothing;
    }

    /// <summary>
    /// Converts corner radius back to control bounds.
    /// Not supported because bounds cannot be calculated from corner radius alone.
    /// </summary>
    /// <param name="value">The corner radius to convert back</param>
    /// <param name="targetType">The target type (Rect)</param>
    /// <param name="parameter">Optional parameters</param>
    /// <param name="culture">Culture information</param>
    /// <exception cref="NotSupportedException">Always thrown - reverse conversion not supported</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}