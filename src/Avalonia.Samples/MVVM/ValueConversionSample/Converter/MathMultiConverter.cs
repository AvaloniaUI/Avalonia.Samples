using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConversionSample.Converter
{
    public class MathMultiConverter : IMultiValueConverter
    {

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 3) throw new ArgumentOutOfRangeException(nameof(values));

            string? operation = values[0] as string;

            double? result = values[1] as double?;

            for (int i = 2; i < values.Count; i++)
            {
                switch (operation)
                {
                    case "+":
                        result += values[i] as double?;
                        break;
                    case "-":
                        result -= values[i] as double?;
                        break;
                    case "*":
                        result *= values[i] as double?;
                        break;
                    case "/":
                        result /= values[i] as double?;
                        break;
                }
            }
            return result;
        }
    }
}
