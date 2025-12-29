using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

public class BoundsToCornerRadiusConverter : IValueConverter
{
    public static BoundsToCornerRadiusConverter Instance { get; } = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Rect bounds)
        {
            var minSize = Math.Min(bounds.Width, bounds.Height);
            return new CornerRadius(minSize / 2);
        }

        return BindingOperations.DoNothing;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}