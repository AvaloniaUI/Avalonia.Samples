using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConversionSample.Converter
{
    /// <summary>
    /// This is a converter which will add two decimal numbers
    /// </summary>
    public class MathAddConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // For Add this is simple. just return the sum of the value and the parameter.
            // You may want to validate value and input in a real world App
            return (double?)value + (double?)parameter;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // If we want to convert back, we need to subtract instead of add.
            return (double?)value - (double?)parameter;
        }
    }
}
