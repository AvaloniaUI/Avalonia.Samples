using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using SharedControls.Controls;

namespace SharedControls.Converters;

/// <summary>
/// Automatically sets the <see cref="HeaderPosition"/> of a <see cref="LabeledControl"/> based on the width of the control.
/// The threshold can be set via the parameter, the default value is 300.
/// </summary>
internal class BoundsToHeaderPositionConverter : IValueConverter
{
    public static BoundsToHeaderPositionConverter Instance { get; } = new BoundsToHeaderPositionConverter();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var threshold = parameter as double? ?? 300;
        
        if (value is Rect bounds)
        {
            return bounds.Width < threshold
                ? HeaderPosition.Top
                : HeaderPosition.Left;
        }
        else
        {
            throw new ArgumentException(null, nameof(value));
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}