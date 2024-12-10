using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnowFlakesControlSample.Models;

/// <summary>
/// This model represents a snowflake
/// </summary>
public partial class SnowFlake
{
    public SnowFlake(double x, double y, double diameter, double speed)
    {
        X = x;
        Y = y;
        Diameter = diameter;
        Radius = diameter / 2.0;
        Speed = speed / 1_000;
    }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1]
    /// </summary>
    public double X { get; private set; }
    
    /// <summary>
    /// Gets the x-position in relative coordinates [0 ... 1]
    /// </summary>
    public double Y { get; private set; }
    
    /// <summary>
    /// Gets the diameter of the snowflake in px.
    /// </summary>
    public double Diameter { get; }

    /// <summary>
    /// Gets the radius of the snowflake in px.
    /// </summary>
    public double Radius { get; }
    
    /// <summary>
    /// Gets the speed of the snowflake in pixel / millisecond
    /// </summary>
    public double Speed { get; }
    
    /// <summary>
    /// Gets the center of the snow flake in absolute coordinates for a given Viewport.
    /// </summary>
    /// <param name="viewport">the viewport info.</param>
    /// <returns>The center point in (px, px)</returns>
    public Point GetCenterForViewport(Rect viewport)
    {
        return new Point(X * viewport.Width + viewport.Left, Y * viewport.Height + viewport.Top);
    }

    public void Move(double elapsedMilliseconds)
    {
        Y += elapsedMilliseconds * Speed;
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