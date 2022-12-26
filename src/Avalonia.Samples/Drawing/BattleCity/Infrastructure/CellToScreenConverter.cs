using System;
using System.Globalization;
using BattleCity.Model;
using Avalonia.Data.Converters;

namespace BattleCity.Infrastructure;

public class CellToScreenConverter : IValueConverter
{
    public static CellToScreenConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ToDouble(value) * GameField.CellSize;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}