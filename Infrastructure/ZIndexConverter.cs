using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using System.Globalization;
using Avalonia.Markup;
using Avalonia.Media;
using Avalonia.BattleCity.Model;
using Avalonia.Data.Converters;

namespace Avalonia.BattleCity.Infrastructure
{
    class ZIndexConverter : IValueConverter
    {
        public static ZIndexConverter Instance { get; } = new ZIndexConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Player)
                return 2;
            if (value is Tank)
                return 1;
            else return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
