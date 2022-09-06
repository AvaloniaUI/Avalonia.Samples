using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

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
            // The first value is the operator and the other two values should be a double.
            if (values.Count != 3)
            {
                // We can write a message into the Trace if we want to inform the developer.
                Trace.WriteLine("Exactly three values expected");
                
                // return "BindingOperations.DoNothing" instead of throwing an Exception.
                // If you want, you can also return a BindingNotification with an Exception
                return BindingOperations.DoNothing;
            }

            // The first item of values is the operation.
            // The operation to use is stored as a string.
            string operation = values[0] as string ?? "+";

            // Create a variable result and assign the first value we have to if
            double value1 = values[1] as double? ?? 0;
            double value2 = values[2] as double? ?? 0;


            // depending on the operator calculate the result.
            switch (operation)
            {
                case "+":
                    return value1 + value2;

                case "-":
                    return value1 - value2;

                case "*":
                    return value1 * value2;

                case "/":
                    // We cannot divide by zero. If value2 is '0', return an error. 
                    if (value2 == 0)
                    {
                        return new BindingNotification(new DivideByZeroException("Don't do this!"), BindingErrorType.Error);
                    }

                    return value1 / value2;
            }

            // If we reach this line, something was wrong. So we return an error notification
            return new BindingNotification(new InvalidOperationException("Something went wrong"), BindingErrorType.Error);
        }
    }
}
