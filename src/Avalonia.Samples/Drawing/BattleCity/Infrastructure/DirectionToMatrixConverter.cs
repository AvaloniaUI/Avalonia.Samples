using System;
using System.Globalization;
using BattleCity.Model;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace BattleCity.Infrastructure;

internal class DirectionToMatrixConverter : IValueConverter
{
    public static DirectionToMatrixConverter Instance { get; } = new();

    // 90 degrees in radians: degrees * (PI / 180) => 90 * PI / 180 = PI / 2.
    private const double QuarterTurnRadians = Math.PI / 2d;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var direction = (Facing)value;
        var matrix = Matrix.Identity;

        // Tank sprite is authored facing north (identity matrix).
        // South can be represented as a vertical flip over the X-axis.
        if (direction == Facing.South) matrix = Matrix.CreateScale(1, -1);

        // East is a +90 degree rotation from north.
        if (direction == Facing.East) matrix = Matrix.CreateRotation(QuarterTurnRadians);

        // West uses east rotation first, then mirrors on the Y-axis (scale X by -1).
        // This keeps the same pivot/origin behavior while producing a left-facing sprite.
        if (direction == Facing.West) matrix = Matrix.CreateRotation(QuarterTurnRadians) * Matrix.CreateScale(-1, 1);

        return new MatrixTransform(matrix);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}