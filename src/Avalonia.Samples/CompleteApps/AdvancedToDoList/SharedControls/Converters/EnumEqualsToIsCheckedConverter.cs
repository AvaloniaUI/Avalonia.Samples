using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

public class EnumEqualsToIsCheckedConverter : IValueConverter
{
    public static EnumEqualsToIsCheckedConverter Instance { get; } = new EnumEqualsToIsCheckedConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var val = value as bool?;
        return val == true ? parameter : BindingOperations.DoNothing;
    }
}