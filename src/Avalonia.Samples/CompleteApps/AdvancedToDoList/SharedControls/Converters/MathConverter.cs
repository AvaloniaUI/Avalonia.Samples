using System.Globalization;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

/// <summary>
/// Multi-purpose mathematical value converter that performs arithmetic operations in XAML bindings.
/// Supports addition, subtraction, multiplication, and division of numeric values.
/// Can work with single values (IValueConverter) or multiple values (IMultiValueConverter).
/// </summary>
/// <remarks>
/// Common use cases:
/// - Adding padding or margins: {Binding Width, Converter={x:Static MathConverter.Add}, ConverterParameter=10}
/// - Subtracting values: {Binding ActualWidth, Converter={x:Static MathConverter.Subtract}, ConverterParameter=20}
/// - Calculating widths: {Binding SomeValue, Converter={x:Static MathConverter.Multiply}, ConverterParameter=2}
/// - Creating percentages: {Binding Width, Converter={x:Static MathConverter.Divide}, ConverterParameter=4}
/// </remarks>
public class MathConverter : IValueConverter, IMultiValueConverter
{
    /// <summary>
    /// Pre-configured converter for addition operations.
    /// Usage: Converter="{x:Static MathConverter.Add}"
    /// </summary>
    public static MathConverter Add { get; } = new MathConverter() { Operator = "+" };

    /// <summary>
    /// Pre-configured converter for subtraction operations.
    /// Usage: Converter="{x:Static MathConverter.Subtract}"
    /// </summary>
    public static MathConverter Subtract { get; } = new MathConverter { Operator = "-" };

    /// <summary>
    /// Pre-configured converter for multiplication operations.
    /// Usage: Converter="{x:Static MathConverter.Multiply}"
    /// </summary>
    public static MathConverter Multiply { get; } = new MathConverter { Operator = "*" };

    /// <summary>
    /// Pre-configured converter for division operations.
    /// Usage: Converter="{x:Static MathConverter.Divide}"
    /// </summary>
    public static MathConverter Divide { get; } = new MathConverter { Operator = "/" };

    /// <summary>
    /// The mathematical operator to use: +, -, *, or /
    /// Set when creating custom converter instances.
    /// </summary>
    public string Operator { get; set; } = "+";

    /// <summary>
    /// Converts a single value by applying the mathematical operator with a parameter.
    /// Implements IValueConverter for two-value operations (value and parameter).
    /// </summary>
    /// <param name="value">The first operand (usually from binding source)</param>
    /// <param name="targetType">The target property type (not used)</param>
    /// <param name="parameter">The second operand (from ConverterParameter in XAML)</param>
    /// <param name="culture">Culture information for string parsing</param>
    /// <returns>Result of applying operator to value and parameter</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Calculate([value, parameter], Operator);
    }

    /// <summary>
    /// Converts a single value back by applying the inverse mathematical operation.
    /// Useful for two-way bindings where you need to reverse the calculation.
    /// </summary>
    /// <param name="value">The value to convert back</param>
    /// <param name="targetType">The target property type (not used)</param>
    /// <param name="parameter">The parameter from binding</param>
    /// <param name="culture">Culture information for string parsing</param>
    /// <returns>Result of applying inverse operation</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Invert the operator for reverse calculation
        var operatorInverted = Operator switch
        {
            "+" => "-",  // To undo addition, subtract
            "-" => "+",  // To undo subtraction, add
            "*" => "/",  // To undo multiplication, divide
            "/" => "*",  // To undo division, multiply
            _ => throw new ArgumentOutOfRangeException(nameof(Operator), Operator, null)
        };

        return Calculate([value, parameter], operatorInverted);
    }

    /// <summary>
    /// Converts multiple values by applying the mathematical operation across all of them.
    /// Implements IMultiValueConverter for operations on multiple binding values.
    /// </summary>
    /// <param name="values">Array of values to operate on (from MultiBinding)</param>
    /// <param name="targetType">The target property type (not used)</param>
    /// <param name="parameter">Additional parameter (usually null for MultiBinding)</param>
    /// <param name="culture">Culture information for string parsing</param>
    /// <returns>Result of applying operator across all values</returns>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return Calculate(values, Operator);
    }

    /// <summary>
    /// Performs the actual mathematical calculation on a list of values.
    /// Handles conversion of various numeric types to double for consistent computation.
    /// Supports int, long, decimal, float, double, and string representations.
    /// </summary>
    /// <param name="values">List of numeric values to calculate</param>
    /// <param name="operatorString">The operator to apply (+, -, *, or /)</param>
    /// <returns>Calculated result as a double, or null if conversion fails</returns>
    private object? Calculate(IList<object?> values, string operatorString)
    {
        // Helper function to convert various types to double for calculation.
        // Handles multiple numeric types and string parsing.
        // 
        // <param name="value">The value to convert to double</param>
        // <returns>Double value or null if conversion fails</returns>
        double? GetDouble(object? value)
        {
            return value switch
            {
                double d => d,                          // Already a double
                string s => double.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : null,  // Parse string
                int i => i,                             // Convert int
                long l => l,                            // Convert long
                decimal d => (double)d,                 // Convert decimal
                float f => (double)f,                   // Convert float
                _ => null                               // Unsupported type
            };
        }

        // Start with the first value
        var result = GetDouble(values.FirstOrDefault());
 

        // Apply operator to each subsequent value
        for (int i =1; i < values.Count; i++)
        {
            var operand = GetDouble(values[i]);
            
            switch (operatorString)
            {
                case "+":
                    result += operand;
                    break;
                case "-":
                    result -= operand;
                    break;
                case "*":
                    result *= operand;
                    break;
                case "/":
                    result /= operand;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operatorString), operatorString, null);
            }
        }

        return result;
    }
}
