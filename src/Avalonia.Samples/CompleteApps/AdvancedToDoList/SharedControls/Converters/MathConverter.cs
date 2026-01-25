using System.Globalization;
using Avalonia.Data.Converters;

namespace SharedControls.Converters;

public class MathConverter : IValueConverter, IMultiValueConverter
{
    public static MathConverter Add { get; } = new MathConverter() { Operator = "+" };
    public static MathConverter Subtract { get; } = new MathConverter { Operator = "-" };
    public static MathConverter Multiply { get; } = new MathConverter { Operator = "*" };
    public static MathConverter Divide { get; } = new MathConverter { Operator = "/" };

    public string Operator { get; set; } = "+";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Calculate([value, parameter], Operator);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var operatorInverted = Operator switch
        {
            "+" => "-",
            "-" => "+",
            "*" => "/",
            "/" => "*",
            _ => throw new ArgumentOutOfRangeException(nameof(Operator), Operator, null)
        };

        return Calculate([value, parameter], operatorInverted);
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return Calculate(values, Operator);
    }

    private object? Calculate(IList<object?> values, string operatorString)
    {
        double? GetDouble(object? value)
        {
            return value switch
            {
                double d => d,
                string s => double.TryParse(s, CultureInfo.InvariantCulture, out var result) ? result : null,
                int i => i,
                long l => l,
                decimal d => (double)d,
                float f => (double)f,
                _ => null
            };
        }

        var result = GetDouble(values.FirstOrDefault());


        for (int i = 1; i < values.Count; i++)
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