using Avalonia.Data.Converters;

namespace ValueConversionSample.Converter;

/// <summary>
/// A static class holding our FuncValueConverter
/// </summary>
/// <remarks>
/// Consume it from XAML via <code>{x:Static conv:FuncValueStringConverters.MyConverter}</code>
/// </remarks>
public static class FuncValueStringConverters
{
    /// <summary>
    /// Gets a Converter that returns true if a string is null or empty, otherwise false
    /// </summary>
    public static FuncValueConverter<string?, bool> IsEmpty { get; } = new FuncValueConverter<string?, bool>(s => string.IsNullOrEmpty(s));
    
    /// <summary>
    /// Gets a Converter that returns true if a string is not null or empty, otherwise false
    /// </summary>
    public static FuncValueConverter<string?, bool> IsNotEmpty { get; } = new FuncValueConverter<string?, bool>(s => !string.IsNullOrEmpty(s));
}