using Avalonia.Data.Converters;

namespace ValueConversionSample.Converter;

/// <summary>
/// A static class holding our FuncValueConverter
/// </summary>
/// <remarks>
/// Consume it from XAML via <code>{x:Static conv:FuncValueMathConverters.MyConverter}</code>
/// </remarks>
public static class FuncValueMathConverters
{
    /// <summary>
    /// Gets a Converter that returns true if a number is 0, otherwise false
    /// </summary>
    public static FuncValueConverter<int, bool> IsZero { get; } = new FuncValueConverter<int, bool>(i => i == 0);
    
    /// <summary>
    /// Gets a Converter that returns true if a number is not 0, otherwise false
    /// </summary>
    public static FuncValueConverter<int, bool> IsNotZero { get; } = new FuncValueConverter<int, bool>(i => i != 0);
}