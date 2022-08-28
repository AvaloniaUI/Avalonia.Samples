using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConversionSample.Converter
{
    /// <summary>
    /// This converter can calculate any number of values. 
    /// </summary>
    public class MathMultiConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // We need to validate if the provided values are valid. We need at least 3 values. 
            // The first value is the operator and additionally at least 2 numbers to add are required.
            if (values.Count < 3)
            {
                // We can write a message into the Trace if we want to inform the user.
                Trace.WriteLine("too less items provided for MathMultiConverter.");

                // return "BindingOperations.DoNothing" instead of throwing an Exception.
                return BindingOperations.DoNothing;
            }

            // The first item if values is the operation.
            // The operation to use is stored as a string.
            string? operation = values[0] as string;

            // Create a varible result and assing the first value we have to if
            double? result = values[1] as double?;

            // loop over all items form i=2 to values.Count
            for (int i = 2; i < values.Count; i++)
            {
                // depedning on the opartor calculate the result.
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

            // finally return the result.
            return result;
        }
    }
}
