using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnowFlakesControlSample.Models;

public partial class SnowFlake
{
    public SnowFlake(double x, double y, double diameter, double speed)
    {
        X = x;
        Y = y;
        Diameter = diameter;
        Radius = diameter / 2.0;
        Speed = speed;
    }

    public double X { get; private set; }
    public double Y { get; private set; }
    public double Diameter { get; }
    public double Speed { get; }


    public double Radius { get; }

    public Point GetCenterForViewport(Rect viewport)
    {
        return new Point(X * viewport.Width + viewport.Left, Y * viewport.Height + viewport.Top);
    }

    public void Move(double elapsedSecons)
    {
        Y += elapsedSecons * Speed;
        if (Y > 1)
        {
            Y = 0;
        }
    }

    public int GetHitScore()
    {
        return (int)(1/Radius * 100 + Speed * 100);
    }

    public bool IsHit(Point point, Rect viewport)
    {
        var distanceSquared = ((Vector)(GetCenterForViewport(viewport) - point)).SquaredLength;

        var radiusSquared = Radius * Radius;

        return distanceSquared <= radiusSquared;
    }
}