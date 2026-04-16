using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using SharedControls.Controls;

namespace SharedControls.Converters;

/// <summary>
/// Value converter that automatically positions control headers based on control width.
/// Implements responsive design by switching header position when control gets too narrow.
/// Useful for mobile devices or narrow windows where space is limited.
/// </summary>
/// <remarks>
/// Typical usage:
/// - Wide controls (>300px): Header on left side (good for desktop)
/// - Narrow controls (≤300px): Header on top (good for mobile)
/// </remarks>
internal class BoundsToHeaderPositionConverter : IValueConverter
{
    /// <summary>
    /// Singleton instance of this converter for use in XAML bindings.
    /// Use this static instance instead of creating new ones for better performance.
    /// </summary>
    public static BoundsToHeaderPositionConverter Instance { get; } = new BoundsToHeaderPositionConverter();
    
    /// <summary>
    /// Converts control bounds to appropriate header position.
    /// Determines whether header should be on top or left based on control width.
    /// </summary>
    /// <param name="value">
    /// The control bounds (Rect) that contains width and height information.
    /// Usually comes from LayoutInformation.GetLayoutSlot() or similar.
    /// </param>
    /// <param name="targetType">
    /// The type of binding target property (should be HeaderPosition).
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <param name="parameter">
    /// Optional threshold value as a double.
    /// If provided, controls switch from left to top header when width smaller than the threshold.
    /// Default is 300 if not provided.
    /// Example in XAML: ConverterParameter="400"
    /// </param>
    /// <param name="culture">
    /// Culture information for localization.
    /// Not used in this converter but required by IValueConverter interface.
    /// </param>
    /// <returns>
    /// HeaderPosition.Top if control width is below the threshold,
    /// HeaderPosition.Left if control width is at or above the threshold.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the value is not a Rect type</exception>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Get threshold from parameter or use default of 300 pixels
        var threshold = parameter as double? ?? 300;
        
        // Only process Rect values
        if (value is Rect bounds)
        {
            // Narrow controls get headers on top, wide controls on left
            return bounds.Width < threshold
                    ? HeaderPosition.Top
                    : HeaderPosition.Left;
        }
        else
        {
            throw new ArgumentException(null, nameof(value));
        }
    }

    /// <summary>
    /// Converts header position back to control bounds.
    /// Not supported because bounds cannot be calculated from header position alone.
    /// </summary>
    /// <param name="value">The header position to convert back</param>
    /// <param name="targetType">The target type (Rect)</param>
    /// <param name="parameter">Optional parameters</param>
    /// <param name="culture">Culture information</param>
    /// <exception cref="NotSupportedException">Always thrown - reverse conversion not supported</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
