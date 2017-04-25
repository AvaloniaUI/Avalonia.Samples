using System;
using System.Globalization;
using Avalonia;
using Avalonia.Markup;
using Avalonia.Media;
using Avalonia.BattleCity.Model;

namespace Avalonia.BattleCity.Infrastructure
{
    class DirectionToMatrixConverter : IValueConverter
    {
        public static DirectionToMatrixConverter Instance { get; } = new DirectionToMatrixConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var direction = (Facing) value;
            var matrix = Matrix.Identity;
            if (direction == Facing.South)
            {
                matrix = Matrix.CreateScale(1, -1);
            }
            if (direction == Facing.East)
            {
                matrix = Matrix.CreateRotation(1.5708);

            }
            if (direction == Facing.West)
            {
                matrix = Matrix.CreateRotation(1.5708) * Matrix.CreateScale(-1, 1);
            }
            return new MatrixTransform(matrix);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
